using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNProvinceResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<GHNProvinceDTO> data { get; set; }
    }
    public class GHNProvinceDTO
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int CountryID { get; set; }
        public string Code { get; set; }
        public List<string> NameExtension { get; set; }
        public int IsEnable { get; set; }
        public int RegionID { get; set; }
        public int RegionCPN { get; set; }
        public int UpdatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool CanUpdateCOD { get; set; }
        public int Status { get; set; }
        public string UpdatedIP { get; set; }
        public int? UpdatedEmployee { get; set; }
        public string UpdatedSource { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
