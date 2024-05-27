namespace DATN_ACV_DEV.Model_DTO.Permission_DTO
{
    public class GetListPermissionResponse
    {
        public GetListPermissionResponse()
        {
            lstPermission = new List<GetListPermissionDTO>();
        }
        public List<GetListPermissionDTO> lstPermission { get; set; }
    }
    public class GetListPermissionDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
    }
}
