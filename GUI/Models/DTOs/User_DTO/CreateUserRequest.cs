using GUI.FileBase;
using System.ComponentModel.DataAnnotations;

namespace GUI.Model_DTO.User_DTO
{
    public class CreateUserRequest : BaseRequest
    {
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
        [Display(Name = "Người dùng")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Chưa nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]

        public string Password { get; set; }
        public string? Position { get; set; }
        [Required(ErrorMessage = "Chưa nhập mã nhân viên")]

        public string UserCode { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên")]
        public string? FullName { get; set; }
        public bool InActive { get; set; } = false;
    }
}
