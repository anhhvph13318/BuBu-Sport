using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNCreateOrderResponse
    {
        public string codeOrderGHN { get; set; }
    }
    public class GHNCreateOrderDTO
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public string district_encode { get; set; }
        public DateTime expected_delivery_time { get; set; }
        public Fee fee { get; set; }
        public string order_code { get; set; }
        public string sort_code { get; set; }
        public string total_fee { get; set; }
        public string trans_type { get; set; }
        public string ward_encode { get; set; }
    }

    public class Fee
    {
        public int coupon { get; set; }
        public int insurance { get; set; }
        public int main_service { get; set; }
        public int r2s { get; set; }
        public int return1 { get; set; }
        public int station_do { get; set; }
        public int station_pu { get; set; }
    }
}
