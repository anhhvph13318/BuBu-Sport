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
    [Route("api/EditCustomer")]
    [ApiController]
    public class EditCustomerController : ControllerBase, IBaseController<EditCustomerRequest, EditCustomerResponse>
    {
        private readonly DBContext _context;

        private EditCustomerRequest _request;
        private BaseResponse<EditCustomerResponse> _res;
        private EditCustomerResponse _response;
        private TbCustomer _Customer;
        private string _apiCode = "EditCustomer";

        public EditCustomerController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditCustomerResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.Id = _Customer.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            try
            {
                _Customer = _context.TbCustomers.Where(p => p.Id == _request.Id).FirstOrDefault();
                if (_Customer != null)
                {
                    _Customer.Name = _request.Name;
                    _Customer.Adress = _request.Adress;
                    _Customer.YearOfBirth = _request.YearOfBirth;
                    _Customer.Sex = _request.Sex;
                    _Customer.UpdateBy =_request.AdminId;//Guid của 1 tài khoản có trong DB
                    _Customer.UpdateDate = DateTime.Now; // Ngày hiện tại 
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
        public BaseResponse<EditCustomerResponse> Process(EditCustomerRequest request)
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
