using GUI.Model_DTO.User_DTO;

namespace GUI.Models.DTOs.User_DTO.Views
{
    public class IndexObject
    {
        public GetListUserResponse Data { get; set; } = new();
        public UserDTO Model { get; set; } = new();
        public string Search { get; set; } = string.Empty;
    }
}
