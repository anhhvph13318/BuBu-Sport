namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class DetailVoucherResponse
    {

        public DetailVoucherResponse()
        {
            voucherDetail = new VoucherDTO();
        }
        public VoucherDTO voucherDetail { get; set; }
    }
}
