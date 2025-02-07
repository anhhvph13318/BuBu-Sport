using GUI.FileBase;
using System.ComponentModel.DataAnnotations;

namespace GUI.Model_DTO.User_DTO
{
    public class EditUserRequest : BaseRequest
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
        [RegularExpression(@"\S+", ErrorMessage = "Tên đăng nhập không được chứa khoảng trắng")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Chưa nhập email")]
        public string Email { get; set; }
        public string? Position { get; set; }
        [Required(ErrorMessage = "Chưa nhập mã nhân viên")]
        public string UserCode { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên")]
        public string FullName { get; set; }
        public bool InActive { get; set; } = false;
        public string? Password { get; set; }

    }
}
