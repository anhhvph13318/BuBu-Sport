using DATN_ACV_DEV.Entity;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Voucher_DTO
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống")]
        public string Code { get; set; } = string.Empty;
        public int Discount { get; set; }
        public string? Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Không thể nhập số âm")]
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public VoucherType Type { get; set; }
        public VoucherUnit Unit { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Không thể nhập số âm")]
        public decimal MaxDiscountAllow { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Không thể nhập số âm")]
        public decimal RequiredTotalAmount { get; set; }
        public IList<Guid> Customers { get; set; } = new List<Guid>();
    }
}