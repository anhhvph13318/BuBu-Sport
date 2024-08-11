using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Voucher_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VoucherController : ControllerBase
    {
        private readonly DBContext _context;

        public VoucherController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVouchers(
            [FromQuery] string? code,
            [FromQuery] string? name,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] VoucherUnit? unit)
        {
            var vouchers = await _context.TbVouchers.AsNoTracking()
                .Where(e => (string.IsNullOrEmpty(code) || e.Code.StartsWith(code.ToUpper()))
                    && (string.IsNullOrEmpty(name) || e.Name.StartsWith(name))
                    && (!startDate.HasValue || e.StartDate >= startDate.Value)
                    && (!endDate.HasValue || e.EndDate <= endDate.Value)
                    && (!unit.HasValue || e.Unit == unit.Value))
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscountAllow = e.MaxDiscountAllow,
                    RequiredTotalAmount = e.RequiredTotalAmount
                }).ToListAsync();

            return Ok(new BaseResponse<GetListVoucherResponse> { Data = new GetListVoucherResponse { LstVoucher = vouchers, TotalCount = vouchers.Count } });
        }

        [HttpGet]
        [Route("search-by")]
        public async Task<IActionResult> GetVoucherByCode([FromQuery] string code)
        {
            var voucher = await _context.TbVouchers.AsNoTracking()
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscountAllow = e.MaxDiscountAllow,
                    RequiredTotalAmount = e.RequiredTotalAmount
                }).FirstOrDefaultAsync(e => e.Code == code.ToUpper());

            if (voucher == null)
                return NotFound();

            return Ok(new BaseResponse<VoucherDTO> { Data = voucher });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherDetail([FromRoute] string id)
        {
            var voucher = await _context.TbVouchers.AsNoTracking()
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscountAllow = e.MaxDiscountAllow,
                    RequiredTotalAmount = e.RequiredTotalAmount
                }).FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (voucher == null)
                return NotFound();

            return Ok(new BaseResponse<VoucherDTO> { Data = voucher });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherDTO request)
        {
            var response = new BaseResponse<CreateVoucherResponse>();

            var isDuplicateVoucherCode = await _context.TbVouchers.AnyAsync(e => e.Code == request.Code.ToUpper());
            if (isDuplicateVoucherCode)
            {
                response.Messages = new List<Message>
                {
                    new() {
                        Field = "Code",
                        MessageText = "Mã voucher đã bị lặp"
                    }
                };

                return BadRequest(response);
            }

            var customer = await _context.TbCustomers
                .AsNoTracking()
                .Select(e => e.Id)
                .ToListAsync();

            var voucher = new TbVoucher
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpper(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Quantity = request.Quantity,
                Unit = request.Unit,
                Status = "0",
                Type = request.Type,
                Discount = request.Discount,
                MaxDiscountAllow = request.MaxDiscountAllow,
                RequiredTotalAmount = request.RequiredTotalAmount,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
                CreateDate = DateTime.Now,
            };

            if(voucher.Type == VoucherType.Evoucher)
            {
                voucher.TbCustomerVouchers = customer.Select(e => new TbCustomerVoucher
                {
                    CustomerId = e,
                    IsUsed = false
                }).ToList();
            }

            await _context.AddAsync(voucher);
            await _context.SaveChangesAsync();

            response.Data = new CreateVoucherResponse { ID = voucher.Id };
            return Ok(response);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucher(
            [FromRoute] string id,
            [FromBody] VoucherDTO request)
        {
            var response = new BaseResponse<EditVoucherResponse>();

            var voucher = await _context.TbVouchers.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));
            if(voucher == null)
                return NotFound(new { id });

            if(request.StartDate > request.EndDate)
            {
                response.Messages.Add(new Message
                {
                    Field = "EndDate",
                    MessageText = "Ngày kết thúc không thể nhỏ hơn ngày bắt đầu"
                });

                return BadRequest(response);
            }

            voucher.Type = request.Type;
            voucher.UpdateDate = DateTime.Now;
            voucher.Code = request.Code;
            voucher.Description = request.Description;
            voucher.Unit = request.Unit;
            voucher.MaxDiscountAllow = request.MaxDiscountAllow;
            voucher.Quantity = request.Quantity;
            voucher.Discount = request.Discount;
            voucher.RequiredTotalAmount = request.RequiredTotalAmount;
            voucher.Name = request.Name;
            voucher.Description = request.Description;

            await _context.SaveChangesAsync();

            response.Data = new EditVoucherResponse { ID = voucher.Id };
            return Ok(response);
        }

        [HttpPost]
        [Route("{code}/apply")]
        public async Task<IActionResult> CanUserApplyVoucher(
            [FromRoute] string code,
            [FromQuery] string? target)
        {
            var voucher = await _context.TbVouchers.AsNoTracking()
                .Where(e => e.StartDate >= DateTime.Now && e.EndDate <= DateTime.Now)
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Type = e.Type,
                    Discount = e.Discount,
                    MaxDiscountAllow = e.MaxDiscountAllow,
                    RequiredTotalAmount = e.RequiredTotalAmount
                })
                .FirstOrDefaultAsync(e => e.Code == code.ToUpper());

            if (voucher is null) return BadRequest();   // check voucher is valid

            if (string.IsNullOrEmpty(target) && voucher.Type == VoucherType.Voucher)    //check voucher is evoucher or not
                return Ok(voucher);

            Guid.TryParse(target, out var customerId);
            var customer = await _context.TbCustomers
                .AsNoTracking()
                .Include(e => e.TbCustomerVoucher)
                .FirstOrDefaultAsync(e => e.Id == customerId || e.Phone == target);

            if (voucher.Type == VoucherType.Voucher && customer is null) // return voucher if voucher is valid and customer didn't buy anything ever
                return Ok(voucher);

            if(customer is null) return BadRequest();

            if(voucher.Type == VoucherType.Voucher && customer.Orders.Any(e => e.VoucherId == voucher.Id))
                return BadRequest();

            var evoucher = customer.TbCustomerVoucher.FirstOrDefault(e => e.VoucherId == voucher.Id);
            if(evoucher is null || evoucher.IsUsed) return BadRequest();

            return Ok(voucher);
        }
    }
}
