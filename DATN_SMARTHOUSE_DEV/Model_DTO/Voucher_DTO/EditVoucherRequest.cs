using DATN_ACV_DEV.FileBase;
using System.Diagnostics.SymbolStore;

namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class EditVoucherRequest: BaseRequest
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Discount { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? Status { get; set; }
        public Guid? ProductID { get; set; }
        public Guid? CategoryID { get; set; }
    }
}
