using DATN_ACV_DEV.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;

namespace DATN_ACV_DEV.FileBase
{
    public class BaseRequest
    {
        private readonly DBContext _context;

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

        public void Authorization(DBContext _context, string _apiCode)
        {
            // Giải mã token JWT

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Utility.Utility.LOGIN_SECRETKEY);
            var token = tokenHandler.ReadJwtToken(Token);
            var Username = token.Payload.Where(a => a.Key == "UserName").Select(x => x.Value).FirstOrDefault().ToString();
            var Id = token.Payload.Where(a => a.Key == "Id").Select(x => x.Value).FirstOrDefault().ToString();
            // Thiết lập các thông số xác thực
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            // Phân quyền chức năng.
            //var user = _context.TbUsers.Where(u => u.Id == Guid.Parse(Id) && u.InActive == true).FirstOrDefault();
            //var groupUser = _context.TbUserGroups.Where(gr => gr.Id == user.UserGroupId && gr.IsDelete == false).FirstOrDefault();
            //checkUserSupperAdmin = groupUser.Code == Utility.Utility.CODE_SUPPER_ADMIN ? true : false;
            //var FuntionForPermissionId = _context.TbUserFuntions.Where(uf => uf.GroupUserId == groupUser.Id).Select(s => s.FuntionForPermissionId);
            //var permissionId = _context.TbFuntionForPermissions.Where(c => FuntionForPermissionId.Contains(c.Id)).Select(s => s.PermissionId);
            //var permission = _context.TbPermissions.Where(a => permissionId.Contains(a.Id)).Select(s => s.Code).ToList();
            //bool checkPermission = !permission.Contains(_apiCode);
            //if (checkPermission && checkUserSupperAdmin != true)
            //{
            //    throw new ACV_Exception
            //    {
            //        Messages = { Message.CreateErrorMessage(_apiCode, _conC401, _conC01Msg, _conC10Field) }
            //    };
            //}
            // ==== phân quyền kết thúc ====
            // Kiểm tra xem thời gian hết hạn đã qua hay chưa
            var expirationTime = token.ValidTo;
            var currentTime = DateTime.UtcNow;
            if (currentTime > expirationTime)
            {
                throw new ACV_Exception
                {
                    Messages = { Message.CreateErrorMessage(_apiCode, _conC401, _conC401Msg, _conC03Field) }
                };
            }
            try
            {
                // Xác thực mã thông báo JWT
                tokenHandler.ValidateToken(Token, tokenValidationParameters, out var validatedToken);
                AdminId = Guid.Parse(Id);
                //Guid result = default(Guid);
                //SiteId = result;
                //ListTAUserId = _listTaId;
            }
            catch
            {
                throw new ACV_Exception
                {
                    Messages = { Message.CreateErrorMessage(_apiCode, _conC401, _conC07Msg, _conC03Field) }
                };
            }


        }
        public void AuthorizationCustomer(DBContext _context, string _apiCode)
        {
            // Giải mã token JWT

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Utility.Utility.LOGIN_CUSTOMER_SECRETKEY);
            var token = tokenHandler.ReadJwtToken(Token);
            var AccountCode = token.Payload.Where(a => a.Key == "AccountCode").Select(x => x.Value).FirstOrDefault().ToString();
            var Id = token.Payload.Where(a => a.Key == "Id").Select(x => x.Value).FirstOrDefault().ToString();
            // Thiết lập các thông số xác thực
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            // Kiểm tra xem thời gian hết hạn đã qua hay chưa
            var expirationTime = token.ValidTo;
            var currentTime = DateTime.UtcNow;
            if (currentTime > expirationTime)
            {
                throw new ACV_Exception
                {
                    Messages = { Message.CreateErrorMessage(_apiCode, _conC401, _conC401Msg, _conC03Field) }
                };
            }
            try
            {
                // Xác thực mã thông báo JWT
                tokenHandler.ValidateToken(Token, tokenValidationParameters, out var validatedToken);
                UserId = Guid.Parse(Id);
                //Guid result = default(Guid);
                //SiteId = result;
                //ListTAUserId = _listTaId;
            }
            catch
            {
                throw new ACV_Exception
                {
                    Messages = { Message.CreateErrorMessage(_apiCode, _conC401, _conC07Msg, _conC03Field) }
                };
            }


        }
    }
}
