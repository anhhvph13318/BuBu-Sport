namespace DATN_ACV_DEV.Model_DTO.User_DTO
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
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Position { get; set; }

        public string? UserCode { get; set; }

        public string? FullName { get; set; }
    }
}
