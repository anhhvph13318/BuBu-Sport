using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.GroupCustomer
{ 
    [Route("api/EditGroupCustomer")] 
    [ApiController]
    public class EditGroupCustomerController : ControllerBase, IBaseController<EditGroupCustomerRequest, EditGroupCustomerResponse>
    {
        private readonly DBContext _context;
        private EditGroupCustomerRequest _request;
        private BaseResponse<EditGroupCustomerResponse> _res;
        private EditGroupCustomerResponse _response;
        private string _apiCode = "EditCategory";
        private TbGroupCustomer _GroupCutomer;
        public EditGroupCustomerController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditGroupCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditGroupCustomerResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.ID = _GroupCutomer.Id;
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
                _GroupCutomer = _context.TbGroupCustomers.Where(c => c.Id == _request.ID && c.IsDelete == false).FirstOrDefault();
                if (_GroupCutomer != null)
                {
                    _GroupCutomer.Name = _request.Name;
                    _GroupCutomer.MinPoint = _request.MinPoint;
                    _GroupCutomer.MaxPoint = _request.MaxPoint;
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
        public BaseResponse<EditGroupCustomerResponse> Process(EditGroupCustomerRequest request)
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
