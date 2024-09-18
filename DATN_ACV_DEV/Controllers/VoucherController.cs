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
                .Where(e => e.Quantity > 0)
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity.Value,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscount = e.MaxDiscount,
                    Status = e.Status == Status.Valid,
                    //RequiredTotalAmount = e.RequiredTotalAmount
                }).ToListAsync();

            return Ok(new BaseResponse<GetListVoucherResponse> { Data = new GetListVoucherResponse { LstVoucher = vouchers, TotalCount = vouchers.Count } });
        }

        [HttpGet]
        [Route("available")]
        public async Task<IActionResult> GetAvailableVoucher([FromQuery] string? phoneNumber)
        {
            var a = await _context.TbVouchers.AsNoTracking()
                .Include(e => e.Orders).ToListAsync();


			var vouchers = await _context.TbVouchers.AsNoTracking()
                .Include(e => e.Orders)
                .Where(e => e.Status == Status.Valid && (string.IsNullOrEmpty(phoneNumber) || !e.Orders.Any(d => d.PhoneNumberCustomer == phoneNumber)))
                .Where(e => e.StartDate <= DateTime.Now)
                .Where(e => e.EndDate >= DateTime.Now)
                .Where(e => e.Quantity > 0)
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity.Value,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscount = e.MaxDiscount,
                    Status = e.Status == Status.Valid,
                    //RequiredTotalAmount = e.RequiredTotalAmount
                }).ToListAsync();

            return Ok(new BaseResponse<GetListVoucherResponse> { Data = new GetListVoucherResponse { LstVoucher = vouchers, TotalCount = vouchers.Count } });
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
                    Quantity = e.Quantity.Value,
                    Unit = e.Unit,
                    Discount = e.Discount,
                    MaxDiscount = e.MaxDiscount,
                    Status = e.Status == Status.Valid,
                    //RequiredTotalAmount = e.RequiredTotalAmount
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

            var voucher = new TbVoucher
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpper(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Quantity = request.Quantity,
                Unit = request.Unit,
                Type = request.Type,
                Discount = request.Discount,
                MaxDiscount = request.MaxDiscount,
                //RequiredTotalAmount = request.RequiredTotalAmount,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
                CreateDate = DateTime.Now,
                Status = request.Status ? Status.Valid : Status.Closed,
            };

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
            voucher.MaxDiscount = request.MaxDiscount;
            voucher.Quantity = request.Quantity;
            voucher.Discount = request.Discount;
            //voucher.RequiredTotalAmount = request.RequiredTotalAmount;
            voucher.Name = request.Name;
            voucher.Description = request.Description;
            voucher.MaxDiscount = request.MaxDiscount;
            voucher.Status = request.Status ? Status.Valid : Status.Closed;
            voucher.EndDate = request.EndDate;
            voucher.StartDate =request.StartDate;

            await _context.SaveChangesAsync();

            response.Data = new EditVoucherResponse { ID = voucher.Id };
            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/apply")]
        public async Task<IActionResult> CanUserApplyVoucher(
            [FromRoute] string id,
            [FromQuery] string? target)
        {
            var voucher = await _context.TbVouchers.AsNoTracking()
                .Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now)
                .Where(e => e.Status == Status.Valid)
                .Where(e => e.Quantity > 0)
                .Select(e => new VoucherDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Quantity = e.Quantity.Value,
                    Unit = e.Unit,
                    Type = e.Type,
                    Discount = e.Discount,
                    MaxDiscount = e.MaxDiscount,
                    Status = e.Status == Status.Valid,
                    //RequiredTotalAmount = e.RequiredTotalAmount
                })
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            if (voucher is null) return BadRequest();   // check voucher is valid

            if (string.IsNullOrEmpty(target))
                return Ok(voucher);

            Guid.TryParse(target, out var customerId);
            var customer = await _context.TbCustomers
                .AsNoTracking()
                .Include(e => e.Orders)
                .FirstOrDefaultAsync(e => e.Id == customerId || e.Phone == target);

            if (customer is not null && customer.Orders.Any(e => e.VoucherId == voucher.Id))
                return BadRequest();

            return Ok(voucher);
        }
    }
}
