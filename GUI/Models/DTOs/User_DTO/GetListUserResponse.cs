using System.ComponentModel;

namespace GUI.Model_DTO.User_DTO
{
    public class GetListUserResponse
    {
        public GetListUserResponse()
        {
            listUser = new List<UserDTO>();
        }
        public List<UserDTO> listUser { get; set; }
        public int TotalCount { get; set; }
    }
    public class UserDTO
    {
        [DisplayName("ID")]
        public Guid Id { get; set; }
        [DisplayName("Tên đăng nhập")]
        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Password { get; set; }
        [DisplayName("Vị trí")]

        public string? Position { get; set; }
        [DisplayName("Mã nhân viên")]

        public string? UserCode { get; set; }
        [DisplayName("Họ tên")]

        public string? FullName { get; set; }
    }
}
