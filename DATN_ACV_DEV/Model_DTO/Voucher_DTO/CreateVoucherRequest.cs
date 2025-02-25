using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;

namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class CreateVoucherRequest : BaseRequest
    {
        [Required(ErrorMessage = "Chưa nhập tên voucher")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Chưa nhập mã voucher")]
        public string Code { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int Discount { get; set; }
        public string? Description { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public VoucherUnit Unit { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal MaxDiscountAllow { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Lấy ngày hôm nay nhưng chỉ lấy phần ngày (không lấy giờ)
            DateTime today = DateTime.Today;

            // Kiểm tra StartDate phải từ hôm nay trở đi
            if (StartDate.Date < today)
            {
                yield return new ValidationResult("Ngày bắt đầu phải là hôm nay hoặc sau", new[] { nameof(StartDate) });
            }

            // Kiểm tra EndDate phải lớn hơn hoặc bằng StartDate
            if (EndDate.Date < StartDate.Date)
            {
                yield return new ValidationResult("Ngày kết thúc không được trước ngày bắt đầu", new[] { nameof(EndDate) });
            }
        }
    }
}
