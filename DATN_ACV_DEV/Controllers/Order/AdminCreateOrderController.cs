using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DATN_ACV_DEV.Controllers.Order.AdminCreateOrderController;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var errors = new Dictionary<string, string>();

            var items = payload.Items.Select(e =>
            {
                var productDetail = _context.TbProductDetails.FirstOrDefault(p => p.Id == Guid.Parse(e.Id));
                var productId = productDetail?.ProductId;
                var product = productId != null ? _context.TbProducts.FirstOrDefault(p => p.Id == productId) : null;

                if (product == null || productDetail == null)
                {
                    errors.Add($"Product.{e.Id}", "Sản phẩm không tồn tại");
                    return null;
                }

                var discount = _context.TbDiscountProducts
                    .Where(dp => dp.ProductId == productId)
                    .Join(_context.TbDiscounts, dp => dp.DiscountId, d => d.Id, (dp, d) => d)
                    .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                    .Select(d => d.DiscountValue)
                    .FirstOrDefault();
                var actualPrice = discount != null ? product.Price - (product.Price * discount / 100) : product.Price;

                return new TbOrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(e.Id),
                    Quantity = e.Quantity,
                    Price = actualPrice ?? 0m 
                };
            }).Where(e => e != null).ToList();

            // Kiểm tra lỗi
            if (errors.Count > 0)
            {
                return BadRequest(new { errors });
            }

            var order = new TbOrder
            {
                Id = Guid.NewGuid(),
                TbOrderDetails = items,
                Status = payload.IsDraft == true ? 0 : payload.Status,
                TotalAmount = payload.Payment.TotalAmount,
                TotalAmountDiscount = payload.Payment.TotalDiscount,
                AmountShip = payload.Payment.ShippingFee,
                IsCustomerTakeYourself = payload.IsCustomerTakeYourSelf,
                IsShippingAddressSameAsCustomerAddress = payload.IsShippingAddressSameAsCustomerAddress,
                CreateDate = DateTime.Now,
                IsDraft = payload.IsDraft,
                PaymentMethod = payload.PaymentMethod,
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

            if (payload.Payment.VoucherId is not null && payload.Payment.VoucherId != Guid.Empty)
            {
                var voucher = await _context.TbVouchers.FirstOrDefaultAsync(e => e.Id == payload.Payment.VoucherId)
                    ?? throw new NullReferenceException();

                if (voucher.Quantity <= 0)
                    errors.Add("Voucher", $"{voucher.Code} - Không hợp lệ");
                else
                {
                    voucher.Quantity -= 1;
                    order.VoucherId = payload.Payment.VoucherId;
                }
            }

            foreach (var item in payload.Items)
            {
                var productID = _context.TbProductDetails.Where(c => c.Id == Guid.Parse(item.Id)).Select(c => c.ProductId).FirstOrDefault();
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == productID)
                    ?? throw new NullReferenceException();

                if (product.Quantity <= 0)
                {
                    errors.Add($"Product.{product.Id}", $"{product.Name} - Không đủ số lượng");
                }
                else
                {
                    if (payload.IsDraft != true)
                    {
                        TbProductDetail tbProductDetail = new TbProductDetail();
                        tbProductDetail = _context.TbProductDetails.Where(c => c.Id == Guid.Parse(item.Id)).FirstOrDefault();
                        tbProductDetail.Quantity -= item.Quantity;
                        product.Quantity -= item.Quantity;
                    }                   
                }
            }

            if (errors.Count == 0)
            {
                try
                {
                    await _context.TbOrders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    return Ok(new { Success = true });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = ex.Message,
                        Detail = ex.InnerException?.Message
                    });
                }
            }

            return BadRequest(new { errors });
        }

        [HttpPatch]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateItemOrder(
        [FromRoute] string id,
        [FromBody] UpdateItemOrderRequest payload)
        {
            var device = 0;
            List<OrderItem> productdetailid = new List<OrderItem>();
            List<TbOrderDetail> newOrderDetails = new List<TbOrderDetail>();
            TbOrder tbOrder = new TbOrder();
            tbOrder = _context.TbOrders.Where(c => c.Id == Guid.Parse(id)).FirstOrDefault();

            if (payload.Items.Select(c=>c.Status).FirstOrDefault() != payload.Status)
            {
                tbOrder.Status = 7;              
            }

            foreach (var item in payload.Items)
            {
                var existItem = _context.TbOrderDetails.Where(e => e.ProductId == Guid.Parse(item.Id) && e.OrderId == Guid.Parse(id)).FirstOrDefault();
                if (existItem == null)
                {
                    var productDetail = _context.TbProductDetails.FirstOrDefault(p => p.Id == Guid.Parse(item.Id));
                    var productId = productDetail?.ProductId;
                    var product = productId != null ? _context.TbProducts.FirstOrDefault(p => p.Id == productId) : null;
                    var actualPrice = product != null ? product.Price : 0m;

                    // Tính giá khuyến mãi nếu có
                    if (product != null)
                    {
                        var discount = _context.TbDiscountProducts
                            .Where(dp => dp.ProductId == productId)
                            .Join(_context.TbDiscounts, dp => dp.DiscountId, d => d.Id, (dp, d) => d)
                            .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                            .Select(d => d.DiscountValue)
                            .FirstOrDefault();
                        actualPrice = discount != null ? product.Price - (product.Price * (decimal)discount / 100) : product.Price;
                    }

                    _context.TbOrderDetails.Add(new TbOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.Parse(item.Id),
                        OrderId = tbOrder.Id,
                        Quantity = item.Quantity,
                        Price = actualPrice
                    });
                    tbOrder.TotalAmount += actualPrice;
                }
                else
                {
                    var productDetail = _context.TbProductDetails.FirstOrDefault(p => p.Id == Guid.Parse(item.Id));
                    var product = productDetail.ProductId != null ? _context.TbProducts.FirstOrDefault(p => p.Id == productDetail.ProductId) : null;
                    if (productDetail.Quantity != 0 && product.Quantity != 0)
                    {
                        if (tbOrder.Status == 7)
                        {
                            productDetail.Quantity -= item.Quantity;
                            product.Quantity -= item.Quantity;
                        }
                        device = 1;
                    }
                }
            }
            foreach (var item1 in productdetailid)
            {
                var productDetail = _context.TbProductDetails.FirstOrDefault(c => c.Id == Guid.Parse(item1.Id));
                var productId = productDetail?.ProductId;
                var product = productId != null ? _context.TbProducts.FirstOrDefault(c => c.Id == productId) : null;
                var actualPrice = product != null ? product.Price : 0m;

                if (product != null)
                {
                    var discount = _context.TbDiscountProducts
                        .Where(dp => dp.ProductId == productId)
                        .Join(_context.TbDiscounts, dp => dp.DiscountId, d => d.Id, (dp, d) => d)
                        .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                        .Select(d => d.DiscountValue)
                        .FirstOrDefault();
                    actualPrice = discount != null ? product.Price - (product.Price * (decimal)discount / 100) : product.Price;
                }

                newOrderDetails.Add(new TbOrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(item1.Id),
                    OrderId = Guid.Parse(id),
                    Quantity = item1.Quantity,
                    Price = actualPrice
                });
            }
            foreach (var item2 in productdetailid)
            {
                var productId = _context.TbProductDetails.Where(c => c.Id == Guid.Parse(item2.Id)).Select(c => c.ProductId).FirstOrDefault();
                var price = _context.TbProducts.Where(c=>c.Id == productId).Select(c => c.Price).FirstOrDefault();
                tbOrder.TotalAmount += price;
            }
            try
            {
                if (productdetailid.Count > 0)
                {
                    await _context.TbOrderDetails.AddRangeAsync(newOrderDetails);
                }
                if (device == 1)
                {
                    _context.TbOrders.Update(tbOrder); // Cập nhật đơn hàng
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                if (device == 0)
                {
                    return StatusCode(400, new
                    {
                        Success = false,
                        Message = "Có sản phẩm đã hết hàng, vui lòng kiểm tra lại !",
                        Detail = "",
                        Data = ""
                    });
                }              
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message,
                    Detail = ex.InnerException?.Message
                });
            }

            return Ok(new { Success = true });
        }
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute] string id,
            [FromBody] UpdateOrder payload)
        {
            var errors = new Dictionary<string, string>();
            var order = await _context.TbOrders
                .Include(e => e.Customer)
                .Include(e => e.AddressDelivery)
                .Include(e => e.TbOrderDetails)
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (order == null) return NotFound();

            order.Status = payload.Status;
            
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
                order.CustomerId = order.CustomerId == null ? payload.Customer.Id : order.CustomerId;
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
            order.IsCustomerTakeYourself = payload.IsCustomerTakeYourSelf;
            order.IsShippingAddressSameAsCustomerAddress = payload.IsShippingAddressSameAsCustomerAddress;

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
            if (payload.Payment.VoucherId is not null && order.VoucherId != Guid.Empty)
            {
                var voucher = await _context.TbVouchers.FirstOrDefaultAsync(e => e.Id == payload.Payment.VoucherId)
                    ?? throw new NullReferenceException();

                voucher.Quantity -= 1;
                order.VoucherId = payload.Payment.VoucherId;
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
            // re-update product stock
            if (payload.Status == 1) // VANH
            {
                foreach (var item in payload.Items)
                {
                    var productId = _context.TbProductDetails.Where(c=>c.Id == Guid.Parse(item.Id)).Select(c=>c.ProductId).FirstOrDefault();
                    var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == productId)
                        ?? throw new NullReferenceException();

                    if (product.Quantity <= 0)
                    {
                        errors.Add($"Product.{product.Id}", $"{product.Name} - Không đủ số lượng");
                    }
                    else
                    {
                        product.Quantity -= item.Quantity;
                    }
                }
            }
            _context.TbOrders.Update(order);
            //_context.SaveChanges();
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message,
                    Detail = ex.InnerException?.Message
                });
            }

            return Ok(new { Success = true });
        }

        [HttpDelete]
        [Route("/api/orders/draft/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var order = await _context.TbOrders
                .Include(e => e.TbOrderDetails)
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id) && e.IsDraft);

            if (order is null)
                return BadRequest();

            foreach (var item in order.TbOrderDetails)
            {
                var productId = _context.TbProductDetails.Where(c => c.Id == item.ProductId).Select(c => c.ProductId).FirstOrDefault();
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == productId)
                    ?? throw new NullReferenceException();

                product.Quantity += item.Quantity;
            }
            _context.TbOrders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public record OrderItem(string Id, int Quantity, int Status);
        public record Order(CustomerInfo Customer,
            IEnumerable<OrderItem> Items,
            ShippingInfo Shipping,
            PaymentInfo Payment,
            int Status,
            bool IsDraft,
            bool IsShippingAddressSameAsCustomerAddress,
            bool IsCustomerTakeYourSelf,
            int PaymentMethod);

        public record UpdateOrder(
            string Id,
            IEnumerable<OrderItem> Items,
            CustomerInfo Customer,
            int Status,
            ShippingInfo Shipping,
            PaymentInfo Payment,
            bool IsDraft,
            bool IsShippingAddressSameAsCustomerAddress,
            bool IsCustomerTakeYourSelf,
            int PaymentMethod) 
            : Order(
                Customer,
                Items,
                Shipping,
                Payment,
                Status,
                IsDraft,
                IsShippingAddressSameAsCustomerAddress,
                IsCustomerTakeYourSelf,
                PaymentMethod);
        public class UpdateItemOrderRequest
        {
            public IList<OrderItem> Items { get; set; }
            public int Status { get; set; }
        }
    }
}
