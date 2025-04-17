using DATN_ACV_DEV.Entity;

namespace GUI.Models.DTOs.Discount_DTO
{
    public class CreateDiscountViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }            // Tên chương trình giảm giá
        public string DiscountType { get; set; }    // Kiểu giảm giá (percent/fixed)
        public decimal? DiscountValue { get; set; }  // Giá trị giảm
        public decimal? MaxDiscountAmount { get; set; } // Số tiền giảm tối đa (chỉ với kiểu giảm theo %)
        public DateTime? StartDate { get; set; }     // Ngày bắt đầu
        public DateTime? EndDate { get; set; }       // Ngày kết thúc

        public List<Guid> ProductIds { get; set; }   // Danh sách ID các sản phẩm được chọn
        public List<TbProduct> Products { get; set; } // Danh sách sản phẩm để hiển thị trong giao diện
    }
}
