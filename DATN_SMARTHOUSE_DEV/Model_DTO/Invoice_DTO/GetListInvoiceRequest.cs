using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Invoice_DTO
{
    public class GetListInvoiceRequest : BaseRequest
    {
        public string? code { get; set; } 
        public DateTime? InputDate { get; set; }
    }
}
