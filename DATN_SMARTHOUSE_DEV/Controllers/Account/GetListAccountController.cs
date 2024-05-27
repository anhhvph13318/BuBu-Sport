using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Account
{
    [Route("api/GetListAccount")] 
    [ApiController]
    public class GetListAccountController : ControllerBase, IBaseController<GetListAccountRequest, GetListAccountResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListAccountRequest _request;
        private BaseResponse<GetListAccountResponse> _res;
        private GetListAccountResponse _response;
        private string _apiCode = "GetListAccount";
        public GetListAccountController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListAccountResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListAccountResponse(); 
        }
        public void AccessDatabase()
        {
            List<AccountDTO> LstAccount = new List<AccountDTO>();
            var Model = _context.TbAccounts.Where(c => (!string.IsNullOrEmpty(_request.Email) ? c.Email.Contains(_request.Email) : true
            && (!string.IsNullOrEmpty(_request.AccountCode) ? c.AccountCode.Contains(_request.AccountCode) : true
            && (!string.IsNullOrEmpty(_request.PhoneNumber) ? c.PhoneNumber.Contains(_request.PhoneNumber) : true)))
            ).OrderByDescending(c => c.CreateDate);
            _response.TotalCount = Model.Count();
            var query = _mapper.Map<List<AccountDTO>>(Model);
            if (_request.Limit == null)
            {
                LstAccount = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstAccount = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstAccount = LstAccount;
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
        public BaseResponse<GetListAccountResponse> Process(GetListAccountRequest request)
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
