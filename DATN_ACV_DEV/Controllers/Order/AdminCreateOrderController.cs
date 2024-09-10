using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Order
{
    [Route("api/orders")]
    [ApiController]
    public class AdminCreateOrderController : ControllerBase
    {
        private readonly DBContext _context;

        public AdminCreateOrderController(DBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Order payload)
        {
            var items = payload.Items.Select(e => new TbOrderDetail
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.Parse(e.Id),
                Quantity = e.Quantity,
            });

            var order = new TbOrder
            {
                Id = Guid.NewGuid(),
                TbOrderDetails = items.ToList(),
                Status = payload.Status,
                TotalAmount = payload.Payment.TotalAmount,
                TotalAmountDiscount = payload.Payment.TotalDiscount,
                AmountShip = payload.Payment.ShippingFee,
                IsCustomerTakeYourself = payload.IsCustomerTakeYourSelf,
                IsShippingAddressSameAsCustomerAddress = payload.IsShippingAddressSameAsCustomerAddress,
                CreateDate = DateTime.Now,
                IsDraft = payload.IsDraft,
                VoucherId = payload.Payment.VoucherId
            };

            var orderCreatedTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            order.OrderCode = payload.IsDraft ? $"TEMP{orderCreatedTime}" : $"OFF{orderCreatedTime}";

            if (payload.Customer.Id != Guid.Empty)
                order.CustomerId = payload.Customer.Id;
            else
            {
                order.Customer = new TbCustomer
                {
                    Id = Guid.NewGuid(),
                    Adress = payload.Customer.Address,
                    Name = payload.Customer.Name,
                    Phone = payload.Customer.PhoneNumber,
                };
            }

            order.PhoneNumberCustomer = payload.Customer.PhoneNumber;

            var hasShippingAddress = !payload.IsCustomerTakeYourSelf && !payload.IsShippingAddressSameAsCustomerAddress;

            if (hasShippingAddress)
            {
                order.AddressDelivery = new TbAddressDelivery
                {
                    Id = Guid.NewGuid(),
                    ProvinceName = payload.Shipping.Address,
                    ReceiverName = payload.Shipping.Name,
                    ReceiverPhone = payload.Shipping.PhoneNumber
                };
            }

            if(order.VoucherId is not null)
            {
                var voucher = await _context.TbVouchers.FirstOrDefaultAsync(e => e.Id == order.VoucherId) 
                    ?? throw new NullReferenceException();

                voucher.Quantity -= 1;
            }

            // re-update product stock
            foreach (var item in payload.Items)
            {
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == Guid.Parse(item.Id))
                    ?? throw new NullReferenceException();

                product.Quantity -= item.Quantity;
            }

            await _context.TbOrders.AddAsync(order);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute] string id,
            [FromBody] UpdateOrder payload)
        {
            var order = await _context.TbOrders
                .Include(e => e.Customer)
                .Include(e => e.AddressDelivery)
                .Include(e => e.TbOrderDetails)
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (order == null) return NotFound();

            order.Status = payload.Status;
            order.VoucherId = payload.Payment.VoucherId;
            
            // update order will be picked-up or delivery
            if (!order.IsCustomerTakeYourself)
            {
                if (payload.IsShippingAddressSameAsCustomerAddress)
                {
                    order.AddressDelivery = null;
                }
                else
                {
                    order.AddressDelivery = new TbAddressDelivery
                    {
                        ReceiverName = payload.Shipping.Name,
                        ReceiverPhone = payload.Shipping.PhoneNumber,
                        ProvinceName = payload.Shipping.Address
                    };

                    order.AmountShip = payload.Shipping.ShippingFee;
                }
            }

            // update order is draft anymore
            order.IsDraft = payload.IsDraft;
            if(order.IsDraft == false)
                order.OrderCode = order.OrderCode!.Replace("TEMP", "OFF");

            // update customer info
            if (payload.Customer.Id != Guid.Empty)
                order.CustomerId = payload.Customer.Id;
            else
            {
                order.Customer = new TbCustomer
                {
                    Id = Guid.NewGuid(),
                    Adress = payload.Customer.Address,
                    Name = payload.Customer.Name,
                    Phone = payload.Customer.PhoneNumber,
                };
            }

            // update phone number
            order.PhoneNumberCustomer = payload.Customer.PhoneNumber;
            order.TotalAmount = payload.Payment.TotalAmount;
            order.TotalAmountDiscount = payload.Payment.TotalDiscount;
            order.AmountShip = payload.Payment.ShippingFee;

            // update shipping address 
            var hasShippingAddress = !payload.IsCustomerTakeYourSelf && !payload.IsShippingAddressSameAsCustomerAddress;
            if (hasShippingAddress)
            {
                order.AddressDelivery = new TbAddressDelivery
                {
                    Id = Guid.NewGuid(),
                    ProvinceName = payload.Shipping.Address,
                    ReceiverName = payload.Shipping.Name,
                    ReceiverPhone = payload.Shipping.PhoneNumber
                };
            }

            // update voucher
            if (order.VoucherId is not null)
            {
                var voucher = await _context.TbVouchers.FirstOrDefaultAsync(e => e.Id == order.VoucherId)
                    ?? throw new NullReferenceException();

                voucher.Quantity -= 1;
            }

            // add and update item has added
            foreach (var item in payload.Items)
            {
                var existItem = order.TbOrderDetails.FirstOrDefault(e => e.ProductId == Guid.Parse(item.Id));
                if (existItem == null)
                    order.TbOrderDetails.Add(new TbOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.Parse(item.Id),
                        Quantity = item.Quantity,
                    });
                else
                    existItem.Quantity = item.Quantity;
            }

            // remove item
            var removeIds = order.TbOrderDetails
                .Select(e => e.ProductId)
                .Except(payload.Items.Select(d => Guid.Parse(d.Id)))
                .ToList();

            foreach(var removeId in removeIds)
            {
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == removeId)
                    ?? throw new NullReferenceException();
                var item = order.TbOrderDetails.First(e => e.ProductId == removeId);

                product.Quantity += item.Quantity;

                order.TbOrderDetails.Remove(item);
            }

            _context.TbOrders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        [HttpDelete]
        [Route("/api/orders/draft/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var order = await _context.TbOrders
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id) && e.IsDraft);
            if (order is not null)
            {
                _context.TbOrders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        public record OrderItem(string Id, int Quantity);
        public record Order(CustomerInfo Customer,
            IEnumerable<OrderItem> Items,
            ShippingInfo Shipping,
            PaymentInfo Payment,
            int Status,
            bool IsDraft,
            bool IsShippingAddressSameAsCustomerAddress,
            bool IsCustomerTakeYourSelf);

        public record UpdateOrder(
            string Id,
            IEnumerable<OrderItem> Items,
            CustomerInfo Customer,
            int Status,
            ShippingInfo Shipping,
            PaymentInfo Payment,
            bool IsDraft,
            bool IsShippingAddressSameAsCustomerAddress,
            bool IsCustomerTakeYourSelf) 
            : Order(
                Customer,
                Items,
                Shipping,
                Payment,
                Status,
                IsDraft,
                IsShippingAddressSameAsCustomerAddress,
                IsCustomerTakeYourSelf);
    }
}
