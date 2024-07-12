using DATN_ACV_DEV.FileBase;
using System.ComponentModel.DataAnnotations;

namespace DATN_ACV_DEV.Model_DTO.User_DTO
{
    public class EditUserRequest : BaseRequest
    {
        public Guid ID { get; set; }
        //public string UserName { get; set; }
        public string Email { get; set; }
        public string? Position { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public bool InActive { get; set; } = false;
    }
}
