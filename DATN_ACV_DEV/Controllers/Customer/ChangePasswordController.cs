using Azure;
using Azure.Core;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [Route("api/ChangePassword")]
    [ApiController]
    public class ChangePasswordController : ControllerBase, IBaseController<ChangePasswordRequest, ChangePasswordResponse>
    {
        private readonly DBContext _context;

        private ChangePasswordRequest _request;
        private BaseResponse<ChangePasswordResponse> _res;
        private ChangePasswordResponse _response;
        private TbCustomer _Customer;
        private TbAccount _Account;
        private string _apiCode = "ChangePassword";

        public ChangePasswordController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<ChangePasswordResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new ChangePasswordResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.Id = _Account?.CustomerId ?? Guid.Empty;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            //_request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            try
            {
                _Account = _context.TbAccounts.FirstOrDefault(c => c.CustomerId == _request.Id);
                if (_Account != null)
                {
                    _Account.Password = _request.Password;
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
        public BaseResponse<ChangePasswordResponse> Process(ChangePasswordRequest request)
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
