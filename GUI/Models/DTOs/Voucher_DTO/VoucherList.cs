namespace GUI.Models.DTOs.Voucher_DTO
{
    public class GetListVoucherRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetListVoucherResponse
    {
        public GetListVoucherResponse()
        {
            LstVoucher = new List<VoucherDTO>();
        }
        public List<VoucherDTO> LstVoucher { get; set; }
        public int TotalCount { get; set; }
    }

    public class VoucherFormModel
    {
        public VoucherDTO Voucher { get; set; } = new VoucherDTO();
        public bool IsEditMode { get; set; }
    }
}