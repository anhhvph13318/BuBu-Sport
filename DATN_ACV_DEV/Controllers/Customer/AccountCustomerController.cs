using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [Route("api/AccountCustomer")]
    [ApiController]
    public class AccountCustomerController : ControllerBase, IBaseController<AccountCustomerRequest, AccountCustomerResponse>
    {
        private readonly DBContext _context;
        private AccountCustomerRequest _request;
        private BaseResponse<AccountCustomerResponse> _res;
        private AccountCustomerResponse _response;
        private string _apiCode = "AccountCustomer";
        private TbCustomer _Customer;
        private TbAccount _Account;
        public AccountCustomerController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<AccountCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Customer = new TbCustomer();
            _response = new AccountCustomerResponse();
            _Account = new();

		}
        public void AccessDatabase()
        {
            try
            {
				_Account = _context.TbAccounts.Where(p => p.Id == _request.Id).FirstOrDefault();
                if (_Account != null)
                {
                    if (_Account.CustomerId == null || _Account.CustomerId == Guid.Empty)
                    {
                        _response.IsCustomer = false;
                    }
                    else
                    {
                        _Customer = _context.TbCustomers.FirstOrDefault(c => c.Id == _Account.CustomerId);
                        if (_Customer != null)
                        {
                            _response.IsCustomer = true;
                            var data = new AccountCustomerDTO
                            {
                                Id = _Customer.Id,
                                Name = _Customer.Name,
                                Adress = _Customer.Adress,
                                Password = _Account.Password,
                                Phone = _Customer.Phone,
                                Sex = _Customer.Sex,
                                YearOfBirth = _Customer.YearOfBirth,
                                Email = _Account.Email,
                            };
                            _response.Customer = data;
                        }
                        else
                        {
                            _response.IsCustomer = false;
                        }
                    }
                }
                else {
                    _response.IsCustomer = true;
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
        public BaseResponse<AccountCustomerResponse> Process(AccountCustomerRequest request)
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
