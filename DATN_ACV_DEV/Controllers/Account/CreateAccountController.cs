using DATN_ACV_DEV.Controllers.Customer;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Account
{
    [Route("api/CreateAccount")] 
    [ApiController]
    public class CreateAccountController : ControllerBase, IBaseController<CreatedAccountRequest, CreatedAccountResponse>
    {
        private readonly DBContext _context;
        private CreatedAccountRequest _request;
        private BaseResponse<CreatedAccountResponse> _res;
        private CreateCustomerRequest _requestCustomer = new CreateCustomerRequest();
        private CreatedAccountResponse _response;
        private string _apiCode = "CreateAccount";
        private TbAccount _Account;
        public CreateAccountController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreatedAccountResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreatedAccountResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_Account);
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
            var phonecheck = _context.TbAccounts.Where(c => c.PhoneNumber == _request.PhoneNumber).Select(c => c.Id).FirstOrDefault();
            if (phonecheck != null)
            {
                #region Tạo mới 1 bản ghi customer ngay khi khách hàng đăng ký
                _requestCustomer.Name = _request.Name;
                var id = new CreateCustomerController(_context).Process(_requestCustomer);
                #endregion
                _Account = new TbAccount()
                {
                    Id = Guid.NewGuid(),
                    AccountCode = _request.AccountCode != null ? _request.AccountCode : "GUEST",
                    Email = _request.Email,
                    Password = _request.Password,
                    PhoneNumber = _request.PhoneNumber,
                    CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
                    CreateDate = DateTime.Now,
                    CustomerId = id.Data.Id,
                };
            }
            else
            {
                _response.Message = "Số điện thoại đăng ký đã được đăng ký, vui lòng đăng ký bằng số khác !!!";
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreatedAccountResponse> Process(CreatedAccountRequest request)
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
