using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.User_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.User
{
    [Route("api/EditUser")]
    [ApiController]
    public class EditUserController : ControllerBase, IBaseController<EditUserRequest, EditUserResponse>
    {
        private readonly DBContext _context;
        private EditUserRequest _request;
        private BaseResponse<EditUserResponse> _res;
        private EditUserResponse _response;
        private TbUser _User;
        public EditUserController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditUserResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditUserResponse();
        }
        public void AccessDatabase()
        {
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
            try
            {
                _User = _context.TbUsers.Where(p => p.Id == _request.ID).FirstOrDefault();
                if (_User != null)
                {
                    _User.UserName = _request.UserName;
                    _User.UserCode = _request.UserCode;
                    _User.Password = _request.Password;
                    _User.Email = _request.Email;
                    _User.FullName = _request.FullName;
                    _User.Position = _request.Position;
                    _User.UpdateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _User.UpdateDate = DateTime.Now; // Ngày hiện tại 
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditUserResponse> Process(EditUserRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation();
                GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
