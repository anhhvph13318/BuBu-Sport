namespace DATN_ACV_DEV.Model_DTO.UserGroup_DTO
{
    public class GetListUserGroupResponse
    {
        public GetListUserGroupResponse()
        {
            lstUserGroup = new List<GetListUserGroupDTO>();
        }
        public List<GetListUserGroupDTO> lstUserGroup { get; set; }
    }
    public class GetListUserGroupDTO
    {
        public Guid id { get; set; }
        public string? name { get; set; }
        public string? code { get; set; }
        public string? description { get; set; }
    }
}
