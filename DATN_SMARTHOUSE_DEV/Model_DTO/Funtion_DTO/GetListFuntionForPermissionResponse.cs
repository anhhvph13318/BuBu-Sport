using DATN_ACV_DEV.Model_DTO.Permission_DTO;

namespace DATN_ACV_DEV.Model_DTO.Funtion_DTO
{
    public class GetListFuntionForPermissionResponse
    {
        public GetListFuntionForPermissionResponse()
        {
            lstPermission = new List<GetListPermissionDTO>();
        }
        public Guid funtionForPermissionId { get; set; }
        public string nameFuntion { get; set; }
        public List<GetListPermissionDTO> lstPermission { get; set; }
    }
}
