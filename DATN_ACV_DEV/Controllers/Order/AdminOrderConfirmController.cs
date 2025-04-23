using DATN_ACV_DEV.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Order
{
    [Route("api/orders")]
    [ApiController]
    public class AdminOrderConfirmController : ControllerBase
    {
        private readonly DBContext _context;

        public AdminOrderConfirmController(DBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder([FromRoute] string id)
        {
            var order = await _context.TbOrders
                .Include(o => o.TbOrderDetails)
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (order is null)
            {
                return NotFound();
            }

            if (order.Status != 4)
            {
                foreach (var item in order.TbOrderDetails)
                {
                    var productId = _context.TbProductDetails
                        .Where(c => c.Id == item.ProductId)
                        .Select(c => c.ProductId)
                        .FirstOrDefault();

                    var product = await _context.TbProducts
                        .FirstOrDefaultAsync(e => e.Id == productId);

                    if (product == null)
                    {
                        return NotFound($"Không tìm thấy sản phẩm với ID: {item.ProductId}");
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        return BadRequest($"Sản phẩm '{product.Name}' không đủ số lượng tồn.");
                    }

                    product.Quantity -= item.Quantity;
                }

                order.Status = 4; // "Xác nhận"
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

    }
}
