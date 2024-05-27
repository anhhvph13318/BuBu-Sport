namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNServiceRequest
    {
        public string token { get; set; }
        public int shopID { get; set; }
        public int fromDistrictID { get; set; }
        public int toDistrictID { get; set; }

    }
    public class GHNServiceReq
    {
        public int shop_id { get; set; }
        public int from_district { get; set; }
        public int to_district { get; set; }
    }
}
