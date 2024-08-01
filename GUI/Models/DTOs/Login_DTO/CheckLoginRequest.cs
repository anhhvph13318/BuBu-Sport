using System.ComponentModel.DataAnnotations;
using GUI.FileBase;

namespace GUI.Models.DTOs.Login_DTO;

public class CheckLoginRequest: BaseRequest
{
   
    [Required(ErrorMessage = "Chưa tên người dùng")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Chưa nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Chưa nhập mật khẩu")]
    public string Password { get; set; }
   
}