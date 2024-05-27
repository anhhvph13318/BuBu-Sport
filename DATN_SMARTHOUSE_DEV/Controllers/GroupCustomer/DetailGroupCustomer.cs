using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.GroupCustomer
{
    [Route("api/DetailGroupCustomer")]
    [ApiController]
    public class DetailGroupCustomer : ControllerBase, IBaseController<DetailGroupCustomerRequest, DetailGroupCustomerResponse>
    {
        private readonly DBContext _context;
        private DetailGroupCustomerRequest _request;
        private BaseResponse<DetailGroupCustomerResponse> _res;
        private DetailGroupCustomerResponse _response;
        private string _apiCode = "DetailGroupCustomer";
        private TbGroupCustomer _GroupCustomer;
        public DetailGroupCustomer(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailGroupCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null,
            };
            _response = new DetailGroupCustomerResponse();
        }
        public void AccessDatabase()
        {
            _GroupCustomer = _context.TbGroupCustomers.Where(c => c.Id == _request.Id && c.IsDelete == false).FirstOrDefault();
            if (_GroupCustomer != null)
            {
                _response.Id = _GroupCustomer.Id;
                _response.Name = _GroupCustomer.Name;
                _response.MinPoint = _GroupCustomer.MinPoint;
                _response.MaxPoint = _GroupCustomer.MaxPoint;
                _response.IsDelete = _GroupCustomer.IsDelete;
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
        public BaseResponse<DetailGroupCustomerResponse> Process(DetailGroupCustomerRequest request)
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
