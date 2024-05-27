using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNFeeRequest
    {
        public int service_type_id { get; set; }
        public int insurance_value { get; set; }
        public string to_ward_code { get; set; }
        public int to_district_id { get; set; }
        public int from_district_id { get; set; }
        public int weight { get; set; }
        public int lenght { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

}
