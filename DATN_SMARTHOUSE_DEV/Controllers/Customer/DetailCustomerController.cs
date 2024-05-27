using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [Route("api/DetailCustomer")]
    [ApiController]
    public class DetailCustomerController : ControllerBase, IBaseController<DetailCustomerRequest, DetailCustomerResponse>
    {
        private readonly DBContext _context;
        private DetailCustomerRequest _request;
        private BaseResponse<DetailCustomerResponse> _res;
        private DetailCustomerResponse _response;
        private string _apiCode = "DetailCustomer";
        private TbCustomer _Customer;
        public DetailCustomerController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Customer = new TbCustomer();
            _response = new DetailCustomerResponse();
        }
        public void AccessDatabase()
        {
            try
            {
                _Customer = _context.TbCustomers.Where(p => p.Id == _request.Id).FirstOrDefault();            
                if (_Customer != null)
                {
                    _response.Id = _Customer.Id;
                    _response.Name = _Customer.Name;
                    _response.Adress = _Customer.Adress;
                    _response.Rank = _Customer.Rank;
                    _response.Status = _Customer.Status;
                    _response.YearOfBirth = _Customer.YearOfBirth;
                    _response.Sex = _Customer.Sex;
                    _response.Point = _Customer.Point;
                    _response.UpdateDate = _Customer.UpdateDate;
                    _response.UpdateBy = _Customer.UpdateBy;
                    _response.GroupCustomerId = _Customer.GroupCustomerId;
                    _response.CreateBy = _Customer.CreateBy;
                    _response.CreateDate = _Customer.CreateDate;
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
        public BaseResponse<DetailCustomerResponse> Process(DetailCustomerRequest request)
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
