using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.UseFuntion_DTO
{
    public class AddUserFuntionRequest : BaseRequest
    {
        public Guid userGroupId { get;set; }
        public List<Guid> funtionForPermissionId { get; set; }
    }
}
