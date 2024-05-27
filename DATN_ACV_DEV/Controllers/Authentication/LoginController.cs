using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DATN_ACV_DEV.Controllers.Authentication
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase, IBaseController<LoginRequest, LoginResponse>
    {
        public IConfiguration _Configuration { get; }
        private readonly DBContext _context;
        private LoginRequest _request;
        private BaseResponse<LoginResponse> _res;
        private LoginResponse _response;
        private string _apiCode = "Login";
        private TbUser _User;
        public LoginController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _res = new BaseResponse<LoginResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new LoginResponse();
            _Configuration = configuration;

        }
        public void AccessDatabase()
        {
            throw new NotImplementedException();
        }

        public void CheckAuthorization()
        {
            _User = _context.TbUsers.Where(u => u.UserName.Trim() == _request.UserName.Trim() && u.Password.Trim() == _request.Password.Trim() && u.InActive == true).FirstOrDefault();
            if (_User == null)
            {
                _response.Message = Utility.Utility.LOGIN_FAIL;
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            if (_User != null)
            {
                string secretKey = _Configuration.GetConnectionString("secretkey");//key code
                byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

                //tạo jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                //    // Thiết lập thông tin xác thực
                var claims = new[]
                {
                    new Claim("Id", _User.Id.ToString()),
                    new Claim("Email", _User.Email),
                    new Claim("UserName", _User.UserName),
                    new Claim("FullName", _User.FullName),
                    new Claim("UserCode", _User.UserCode),
                };

                //    // thời gian token tồn tại
                int timeToken = Convert.ToInt32(_Configuration.GetConnectionString("SetTimeToken"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMilliseconds(timeToken),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha512)
                };
                // Tạo token JWT
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                _response.Message = Utility.Utility.LOGIN_DONE;
                _response.UserName = _User.UserName;
                _response.Id = _User.Id;
                _response.Token = tokenString;
            }
            _res.Data = _response;
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<LoginResponse> Process(LoginRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
                //GenerateObjects();
                //PostValidation();
                //AccessDatabase();
            }
            catch (ACV_Exception ex)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex.Messages;
            }
            catch (System.Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;
        }
    }
}
