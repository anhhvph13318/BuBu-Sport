using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.User_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.User
{
    [Route("api/DetailUser")]
    [ApiController]
    public class DetailUserController : ControllerBase, IBaseController<DetailUserRequest, DetailUserResponse>
    {
        private readonly DBContext _context;
        private DetailUserRequest _request;
        private BaseResponse<DetailUserResponse> _res;
        private DetailUserResponse _response;
        private string _apiCode = "DetailUser";
        private TbUser _User;
        public DetailUserController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailUserResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _User = new TbUser();
            _response = new DetailUserResponse();
        }
        public void AccessDatabase()
        {
            try
            {
                _User = _context.TbUsers.Where(p => p.Id == _request.Id && p.InActive == false).FirstOrDefault();
                if (_User != null)
                {
                    _response.Id = _User.Id;
                    _response.UserName = _User.UserName;
                    _response.Email = _User.Email;
                    _response.Password = _User.Password;
                    _response.Position = _User.Position;
                    _response.UserCode = _User.UserCode;
                    _response.FullName = _User.FullName;
                    _response.InActive = _User.InActive;

                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
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
        public BaseResponse<DetailUserResponse> Process(DetailUserRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation();
                //GenerateObjects();
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
