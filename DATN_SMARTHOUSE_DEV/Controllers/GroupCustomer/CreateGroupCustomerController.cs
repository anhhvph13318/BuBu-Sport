using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.GroupCustomer
{
    [Route("api/CreateGroupCustomer")] 
    [ApiController]
    public class CreateGroupCustomerController : ControllerBase, IBaseController<CreateGroupCustomerRequest, CreateGroupCustomerResponse>
    {
        private readonly DBContext _context;
        private CreateGroupCustomerRequest _request;
        private BaseResponse<CreateGroupCustomerResponse> _res;
        private CreateGroupCustomerResponse _response;
        private TbGroupCustomer _Customer;
        public CreateGroupCustomerController(DBContext context) 
        {
            _context = context;
            _res = new BaseResponse<CreateGroupCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null,
            };
            _response = new CreateGroupCustomerResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_Customer);
            _context.SaveChanges();
            _response.ID = _Customer.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Customer = new TbGroupCustomer()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                MinPoint = _request.MinPoint,
                MaxPoint = _request.MaxPoint,
                IsDelete = false,
            };
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateGroupCustomerResponse> Process(CreateGroupCustomerRequest request)
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
