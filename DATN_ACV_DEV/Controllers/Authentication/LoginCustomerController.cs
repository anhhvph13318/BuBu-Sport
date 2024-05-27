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
    [Route("api/LoginCustomer")]
    [ApiController]
    public class LoginCustomerController : ControllerBase, IBaseController<LoginCustomerRequest, LoginCustomerResponse>
    {
        public IConfiguration _Configuration { get; }
        private readonly DBContext _context;
        private LoginCustomerRequest _request;
        private BaseResponse<LoginCustomerResponse> _res;
        private LoginCustomerResponse _response;
        private string _apiCode = "LoginCustomer";
        private TbAccount _Account;
        public LoginCustomerController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _res = new BaseResponse<LoginCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new LoginCustomerResponse();
            _Configuration = configuration;

        }
        public void AccessDatabase()
        {
            throw new NotImplementedException();
        }

        public void CheckAuthorization()
        {
            _Account = _context.TbAccounts.Where(u => u.PhoneNumber.Trim() == _request.PhoneNumber.Trim() && u.Password.Trim() == _request.Password.Trim() /*&& u.Customer.Status == Utility.Utility.CUSTOMER_ACTIVE*/).FirstOrDefault();
            if (_Account == null)
            {
                _response.Message = Utility.Utility.LOGIN_FAIL;
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            if (_Account != null)
            {
                string secretKey = _Configuration.GetConnectionString("secretkeyCustomer");//key code
                byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

                //tạo jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                //    // Thiết lập thông tin xác thực
                var claims = new[]
                {
                    new Claim("Id", _Account.Id.ToString()),
                    new Claim("Email", _Account.Email),
                    new Claim("AccountCode", _Account.AccountCode),
                    new Claim("PhoneNumber", _Account.PhoneNumber),
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
                _response.UserName = _Account.AccountCode;
                _response.Id = _Account.Id;
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
        public BaseResponse<LoginCustomerResponse> Process(LoginCustomerRequest request)
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
