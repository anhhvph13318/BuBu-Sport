using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.AddressDelivery;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.User_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.User
{
    [Route("api/GetListUser")]
    [ApiController]
    public class GetListUserController : ControllerBase, IBaseController<GetListUserRequest, GetListUserResponse>
    {
        private readonly DBContext _context;
        private GetListUserRequest _request;
        private BaseResponse<GetListUserResponse> _res;
        private GetListUserResponse _response;
        private readonly IMapper _mapper;
        private string _apiCode = "GetListUser";
        public GetListUserController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetListUserResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListUserResponse();
            _mapper = mapper;
        }
        public void AccessDatabase()
        {
            List<UserDTO> LstUser = new List<UserDTO>();
            var model = _context.TbUsers.Where(c => (!string.IsNullOrEmpty(_request.UserName) ? c.UserName.Contains(_request.UserName) : true)
                        && (!string.IsNullOrEmpty(_request.FullName) ? c.FullName == _request.FullName : true)
                        && (!string.IsNullOrEmpty(_request.Position) ? c.Position.ToString() == _request.Position : true)
                        && (!string.IsNullOrEmpty(_request.Email) ? c.Email.ToString() == _request.Email : true)
                        && (!string.IsNullOrEmpty(_request.UserCode) ? c.UserCode.ToString() == _request.UserCode : true)
                        ).OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var query = _mapper.Map<List<UserDTO>>(model);
            if (_request.Limit == null)
            {
                LstUser = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstUser = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.listUser = LstUser;
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
        public BaseResponse<GetListUserResponse> Process(GetListUserRequest request)
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
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
