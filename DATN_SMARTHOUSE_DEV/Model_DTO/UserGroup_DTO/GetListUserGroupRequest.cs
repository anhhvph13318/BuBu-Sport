using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.UserGroup_DTO
{
    public class GetListUserGroupRequest : BaseRequest
    {
        public string? searchKey { get; set; }
    }
}
