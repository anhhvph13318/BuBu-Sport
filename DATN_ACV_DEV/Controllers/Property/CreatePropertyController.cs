using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Property
{
    [Route("api/CreateProperty")]
    [ApiController]
    public class CreatePropertyController : ControllerBase, IBaseController<CreatePropertyRequest, CreatePropertyResponse>
    {
        private readonly DBContext _context;
        private CreatePropertyRequest _request;
        private BaseResponse<CreatePropertyResponse> _res;
        private CreatePropertyResponse _response;
        private TbProperty _Property;
        public CreatePropertyController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreatePropertyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreatePropertyResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_Property);
            _context.SaveChanges();
            _response.Message = "Thêm mới thành công";
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Property = new TbProperty()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Active = true,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"), // Tạm thời gán guid khởi tạo.
                CreateDate = DateTime.Now, // Ngày hiện tại 
                ProductId = _request.ProductId,
                CategoryId = _request.CategoryId,
            };
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreatePropertyResponse> Process(CreatePropertyRequest request)
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
