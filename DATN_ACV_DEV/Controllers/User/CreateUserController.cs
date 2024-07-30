using Azure;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.User_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.User
{
    [Route("api/CreateUser")]
    [ApiController]
    public class CreateUserController : ControllerBase, IBaseController<CreateUserRequest, CreateUserResponse>
    {
        private readonly DBContext _context;
        private CreateUserRequest _request;
        private BaseResponse<CreateUserResponse> _res;
        private CreateUserResponse _response;
        private string _apiCode = "CreateUser";
        private TbUser _User;
        public CreateUserController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateUserResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateUserResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_User);
            _context.SaveChanges();
            _response.ID = _User.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _User = new TbUser()
            {
                Id = Guid.NewGuid(),
                UserName = _request.UserName,
                Email = _request.Email,
                Password = _request.Password,
                Position = _request.Position,
                UserCode = _request.UserCode,
                FullName = _request.FullName,
                InActive = _request.InActive,
                CreateDate = DateTime.Now,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
            };
        }

        public void PreValidation()
        {
            var check = _context.TbUsers.Any(u => u.UserName == _request.UserName);
            if (check) {
                throw new ACV_Exception() { Messages = new List<Message>() { new Message() { MessageText = "Tên đăng nhập đã tồn tại" } } };
            }
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateUserResponse> Process(CreateUserRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                PreValidation();
                GenerateObjects();
                //PostValidation();
                AccessDatabase();
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
