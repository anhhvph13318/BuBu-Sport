﻿using DATN_ACV_DEV.Entity;
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
                OrderCode = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss").Replace("-", ""),
                CreateDate = DateTime.Now,
            };

            if(payload.Customer.Id != Guid.Empty)
                order.CustomerId = payload.Customer.Id;
            else
            {
                var tempGroup = await _context.TbGroupCustomers.FirstOrDefaultAsync();
                order.Customer = new TbCustomer
                {
                    Id = Guid.NewGuid(),
                    Adress = payload.Customer.Address,
                    Name = payload.Customer.Name,
                    Phone = payload.Customer.PhoneNumber,
                    GroupCustomer = tempGroup!
                };
            }

            var hasShippingAddress = !payload.IsCustomerTakeYourSelf && !payload.IsShippingAddressSameAsCustomerAddress;

            if(hasShippingAddress)
            {
                order.AddressDelivery = new TbAddressDelivery
                {
                    Id = Guid.NewGuid(),
                    ProvinceName = payload.Shipping.Address,
                    ReceiverName = payload.Shipping.Name,
                    ReceiverPhone = payload.Shipping.PhoneNumber
                };
            }

            // re-update product stock
            foreach(var item in payload.Items)
            {
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == Guid.Parse(item.Id))
                    ?? throw new NullReferenceException();

                product.Quantity -= item.Quantity;
            }

            await _context.TbOrders.AddAsync(order);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        public record OrderItem(string Id, int Quantity);
        public record Order(CustomerInfo Customer,
            IEnumerable<OrderItem> Items,
            ShippingInfo Shipping,
            PaymentInfo Payment,
            int Status,
            bool IsShippingAddressSameAsCustomerAddress,
            bool IsCustomerTakeYourSelf);
    }
}
