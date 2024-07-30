using DATN_ACV_DEV.Entity;
using Microsoft.AspNetCore.Http;
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
            var tempGroup = await _context.TbGroupCustomers.FirstOrDefaultAsync();
            var customer = new TbCustomer
            {
                Id = Guid.NewGuid(),
                Adress = payload.Customer.Address,
                Name = string.IsNullOrEmpty(payload.Customer.Name) ? "Khách vãng lai" : payload.Customer.Name,
                GroupCustomer = tempGroup!
            };

            if (!string.IsNullOrEmpty(payload.Customer.Id))
            {
                customer = await _context.TbCustomers.FirstOrDefaultAsync(e => e.Id == Guid.Parse(payload.Customer.Id));
            }

            var items = payload.Items.Select(e => new TbOrderDetail
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.Parse(e.Id),
                Quantity = e.Quantity,
            });

            var order = new TbOrder
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                TbOrderDetails = items.ToList(),
                Status = 7,
                TotalAmount = payload.TotalAmount,
                TotalAmountDiscount = payload.DiscountAmount,
                OrderCode = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss").Replace("-", ""),
                CreateDate = DateTime.Now,
            };

            // re-update product stock
            foreach (var item in payload.Items)
            {
                var product = await _context.TbProducts.FirstOrDefaultAsync(e => e.Id == Guid.Parse(item.Id))
                    ?? throw new NullReferenceException();

                product.Quantity -= item.Quantity;
            }

            await _context.TbOrders.AddAsync(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public record OrderItem(string Id, int Quantity);
        public record Customer(string Id, string Name, string PhoneNumber, string Address);
        public record Order(Customer Customer, IEnumerable<OrderItem> Items, decimal TotalAmount, decimal DiscountAmount);
    }
}