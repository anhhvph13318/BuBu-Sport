using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNWardResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<GHNWardDTO> data { get; set; }
    }
    public class GHNWardDTO
    {
        public string WardCode { get; set; }
        public int DistrictID { get; set; }
        public string WardName { get; set; }
        public List<string> NameExtension { get; set; }
        public bool CanUpdateCOD { get; set; }
        public int SupportType { get; set; }
        public int PickType { get; set; }
        public int DeliverType { get; set; }
        public WhiteListClient WhiteListClient { get; set; }
        public WhiteListWard WhiteListWard { get; set; }
        public int Status { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public object OnDates { get; set; }
        public string CreatedIP { get; set; }
        public int CreatedEmployee { get; set; }
        public string CreatedSource { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedEmployee { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? IsEnable { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
    public class WhiteListWard
    {
        public object From { get; set; }
        public object To { get; set; }
    }
}
