using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Account
{
    [Route("api/EditAccount")] 
    [ApiController]
    public class EditAccountController : ControllerBase, IBaseController<EditAccountRequest, EditAccountResponse>
    {
        private readonly DBContext _context;
        private EditAccountRequest _request;
        private BaseResponse<EditAccountResponse> _res;
        private EditAccountResponse _response;
        private string _apiCode = "EditAccount";
        private TbAccount _Account;
        public EditAccountController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditAccountResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditAccountResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.ID = _Account.Id;
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
                _Account = _context.TbAccounts.Where(c => c.Id == _request.ID).FirstOrDefault();
                if (_Account != null)
                {
                    _Account.AccountCode = _request.AccountCode != null ? _request.AccountCode : _Account.AccountCode;
                    _Account.PhoneNumber = _request.PhoneNumber != null ? _request.PhoneNumber : _Account.PhoneNumber;
                    _Account.Email = _request.Email != null ? _request.Email : _Account.Email;
                    _Account.Password = _request.Password != null ? _request.Password : _Account.Password;
                    _Account.UpdateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Account.UpdateDate = DateTime.Now; // Ngày hiện tại 
                }
            }
            catch (Exception)
            {

                _res.Status = StatusCodes.Status400BadRequest.ToString(); ;
            }      
        }
        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditAccountResponse> Process(EditAccountRequest request)
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
