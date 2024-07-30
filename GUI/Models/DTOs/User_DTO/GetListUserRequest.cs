using GUI.FileBase;

namespace GUI.Model_DTO.User_DTO
{
    public class GetListUserRequest : BaseRequest
    {
        public string? UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Position { get; set; }

        public string? UserCode { get; set; }

        public string? FullName { get; set; }
    }
}
