namespace DATN_ACV_DEV.Model_DTO.Policy_DTO
{
    public class GetListPolicyResponse
    {
        public GetListPolicyResponse()
        {
            ListPolicy = new List<PolicyDTO>();
        }
        public List<PolicyDTO> ListPolicy { get; set; }
        public int TotalCount { get; set; }
    }
}
