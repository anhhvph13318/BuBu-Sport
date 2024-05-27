namespace DATN_ACV_DEV.Model_DTO.Funtion_DTO
{
    public class GetListFuntionResponse
    {
        public GetListFuntionResponse()
        {
            listFuntion = new List<GetListFuntionDTO>();
        }
        public List<GetListFuntionDTO> listFuntion { get; set; }
    }
    public class GetListFuntionDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
    }
}
