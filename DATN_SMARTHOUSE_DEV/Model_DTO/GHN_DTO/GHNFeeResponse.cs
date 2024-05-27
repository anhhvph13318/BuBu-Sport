namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNFeeResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public GHNFeeDTO data { get; set; }
    }
    public class GHNFeeDTO
    {
        public int total { get; set; }
        public int service_fee { get; set; }
        public int insurance_fee { get; set; }
        public int pick_station_fee { get; set; }
        public int coupon_value { get; set; }
        public int r2s_fee { get; set; }
    }
}
