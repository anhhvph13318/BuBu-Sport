using DATN_ACV_DEV.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [ApiController]
    [Route("/api/customers")]
    public class GetBasicCustomerInfoController
    {
        private readonly DBContext _context;

        public GetBasicCustomerInfoController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetBasicInfo([FromRoute] int id)
        {
            //var customer = await _context.TbCustomers
            //    .AsNoTracking()
            //    .Select(e => new
            //    {
            //        e.Id,
            //        e.Name,
            //        e.
            //    })
            throw new NotImplementedException();
        }
    }
}
