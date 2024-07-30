
using System.Text.Json.Serialization;

namespace GUI.FileBase
{
    public class BaseRequest
    {

        private string _conC01 = "C01";

        private string _conC02 = "C02";

        private string _conC401 = "C401";

        private string _conC05 = "C05";

        private string _conC10 = "C10";

        private string _conC07Msg = "Token không hợp lệ";

        private string _conC01Msg = "Bạn chưa được cấp quyền truy cập!";

        private string _conC02Msg = "Token không hợp lệ";

        private string _conC401Msg = "Bạn đã hết thời gian truy cập. Vui lòng đăng nhập lại";

        private string _conC05Msg = "Bạn chưa chọn Site làm việc";

        private string _conC10Msg = "Bạn không có quyền thực hiện chức năng này!";

        private string _conC01Field = "Token";

        private string _conC02Field = "Token";

        private string _conC03Field = "Token";

        private string _conC05Field = "SiteId";

        private string _conC10Field = "Permission";
        private bool checkUserSupperAdmin = false;

        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid? AdminId { get; set; }
        public bool LoginType { get; set; }
        public string? Token { get; set; }
        public int? OffSet { get; set; } = 0;
        public int? Limit { get; set; }

    }
}
