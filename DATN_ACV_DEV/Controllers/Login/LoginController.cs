
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Login;


    [Route("api/Login")] 
    [ApiController]
    public class LoginController : ControllerBase, IBaseController<LoginRequest, LoginResponse>
    {
        private readonly DBContext _context;
        private LoginRequest _request;
        private BaseResponse<LoginResponse> _res;
        private LoginResponse _response;
        private string _apiCode = "Login";
        private TbUser _user;
        public LoginController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<LoginResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new LoginResponse();
        }
        [NonAction]
        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }
        [NonAction]
        public void AccessDatabase()
        {
            _context.Add(_user);
            _context.SaveChanges();
            _response.Id= _user.Id;
            _res.Data = _response;
        }
        [HttpPost("check-login")]
        public  BaseResponse<LoginResponse> Process([FromBody] LoginRequest request)
        { 
            try
            {
                var user = _context.TbUsers.FirstOrDefault(c =>
                    c.UserName.ToLower() == request.UserName && c.Password == request.Password);
                if( user is not null) 
                {
                    _res.Status = StatusCodes.Status200OK.ToString();
                    _res.Messages= new List<Message>() { new Message() { MessageText = user.Id.ToString() } } ;
                    return _res;
                }
                else
                {
                    _res.Status = StatusCodes.Status404NotFound.ToString();
                    _res.Messages = new List<Message>() { new Message() { MessageText = "Tên đăng nhập hoặc mật khẩu không đúng! Xin vui lòng thử lại." } } ;
                    return _res;
                }
            }
            catch (ACV_Exception ex)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex.Messages;
                return _res;
            }
            catch (System.Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
                return _res;
            }
        
        }
     
        [NonAction]
        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }
        [NonAction]
        public void PreValidation()
        {
            var check = _context.TbUsers.Any(u => u.UserName == _request.UserName);
            if (check) {
                throw new ACV_Exception() { Messages = new List<Message>() { new Message() { MessageText = "Tên đăng nhập hoặc mật khẩu không chính xác! Xin vui lòng thử lại." } } };
            }
        }
    }
