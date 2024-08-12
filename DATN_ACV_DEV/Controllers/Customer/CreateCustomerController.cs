using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [Route("api/CreateCustomer")]
    [ApiController]
    public class CreateCustomerController : ControllerBase, IBaseController<CreateCustomerRequest, CreateCustomerResponse>
    {
        private readonly DBContext _context;
        private CreateCustomerRequest _request;
        private BaseResponse<CreateCustomerResponse> _res;
        private CreateCustomerResponse _response;
        private TbCustomer _Customer;
        private string _apiCode = "CreateCustomer";
        public CreateCustomerController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateCustomerResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_Customer);
            _context.SaveChanges();
            _response.message = "Thêm mới thành công !!!";
            _response.Id = _Customer.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
            //_request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            _Customer = new TbCustomer()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Adress = _request.Adress,
                Rank = _request.Rank,
                Status = _request.Status,
                YearOfBirth = _request.YearOfBirth,
                Sex = _request.Sex,
                Point = _request.Point,
                UpdateDate = _request.UpdateDate,
                UpdateBy = _request.UpdateBy,
                CreateDate = DateTime.Now,
                //Default
                CreateBy = _request.AdminId, // Tạm thời gán guid khởi tạo.

            };
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateCustomerResponse> Process(CreateCustomerRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation(); // validate dữ liệu 
                GenerateObjects(); // Gán dữ liệu 
                AccessDatabase(); // Lưu xuống DB 
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
