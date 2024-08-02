using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [ApiController]
    [Route("/api/customers")]
    public class GetBasicCustomerInfoController : ControllerBase
    {
        private readonly DBContext _context;

        public GetBasicCustomerInfoController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{phone}")]
        public async Task<IActionResult> GetBasicInfo([FromRoute] string phone)
        {
            var customer = await _context.TbCustomers
                .AsNoTracking()
                .Select(e => new CustomerInfo
                {
                    Id = e.Id,
                    Name = e.Name,
                    Address = e.Adress,
                    PhoneNumber = e.Phone
                }).FirstOrDefaultAsync(e => e.PhoneNumber == phone);

            return Ok(new BaseResponse<CustomerInfo>()
            {
                Data = customer ?? new CustomerInfo()
            });
        }
    }
}
