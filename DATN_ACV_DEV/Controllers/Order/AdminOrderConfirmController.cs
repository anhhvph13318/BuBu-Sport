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
            var order = await _context.TbOrders.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (order is null)
            {
                return NotFound();
            }

            order.Status = 7;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
