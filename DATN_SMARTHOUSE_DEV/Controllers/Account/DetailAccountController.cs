using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Account
{
    [Route("api/DetailAccount")]
    [ApiController]
    public class DetailAccountController : ControllerBase, IBaseController<DetailAccountRequest, DetailAccountResponse>
    {
        private readonly DBContext _context;
        private DetailAccountRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<DetailAccountResponse> _res;
        private DetailAccountResponse _response;
        private string _apiCode = "DetailAccount";
        private TbAccount _Account;
        public DetailAccountController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailAccountResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Account = new TbAccount();
            _response = new DetailAccountResponse();
        }
        public void AccessDatabase()
        {
            try
            {
                _Account = _context.TbAccounts.Where(p => p.Id == _request.Id).FirstOrDefault();
                if (_Account != null)
                {
                    _response.Id = _Account.Id;
                    _response.AccountCode = _Account.AccountCode;
                    _response.Email = _Account.Email;
                    _response.PhoneNumber = _Account.PhoneNumber;
                    _response.Password = _Account.Password;
                    _response.CustomerID = _Account.CustomerId;
                    _response.Name = _Account.CustomerId != null ? _context.TbCustomers.Where(c => c.Id == _Account.CustomerId).Select(c => c.Name).FirstOrDefault() : null;
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
        public BaseResponse<DetailAccountResponse> Process(DetailAccountRequest request)
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
