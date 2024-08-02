using DATN_ACV_DEV.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Product
{
    [Route("api/products")]
    [ApiController]
    public class CheckProductStockController : ControllerBase
    {
        private readonly DBContext _context;

        public CheckProductStockController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{id}/stock")]
        public async Task<IActionResult> GetProductStock([FromRoute] string id)
        {
            var product = await _context.TbProducts
                .AsNoTracking()
                .Select(e => new
                {
                    e.Id,
                    Stock = e.Quantity
                })
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            return product is null
                ? NotFound()
                : Ok(product);
        }
    }
}
