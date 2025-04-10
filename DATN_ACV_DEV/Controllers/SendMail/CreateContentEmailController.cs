using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.SendEmail_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.SendMail
{
    [Route("api/CreateContentEmail")]
    [ApiController]
    public class CreateContentEmailController : ControllerBase, IBaseController<CreateEmailRequest, CreateEmailResponse>
    {
        private readonly DBContext _context;
        private CreateEmailRequest _request;
        private BaseResponse<CreateEmailResponse> _res;
        private CreateEmailResponse _response;
        private string _apiCode = "CreateEmail";
        private TbAccount _Account;
        public CreateContentEmailController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateEmailResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateEmailResponse();
        }

        public void AccessDatabase()
        {
            _res.Data = _response;
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Account = _context.TbAccounts.Where(c => c.Email == _request.Email).FirstOrDefault();
            if (_Account != null)
            {
                _response.Email = _Account.Email;
                _response.phonenumber = _Account.PhoneNumber;
                _response.customerName = _context.TbCustomers.Where(c => c.Id == _Account.CustomerId).Select(c => c.Name).FirstOrDefault();
                _response.password = _request.password;
                _Account.Password = _response.password;
            }
            else {

                _response.Message = "Email chưa được đăng ký tài khoản, vui lòng kiểm tra lại !!!";
            }
            
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateEmailResponse> Process(CreateEmailRequest request)
        {
            try
            {
                _request = request;
                GenerateObjects();
                AccessDatabase();
            }
            catch (Exception)
            {

                throw;
            }
            return _res;
        }
    }
}
