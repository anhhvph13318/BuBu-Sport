using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Property
{
    [Route("api/EditProperty")]
    [ApiController]
    public class EditPropertyController : ControllerBase, IBaseController<EditPropertyRequest, EditPropertyResponse>
    {
        private readonly DBContext _context;
        private EditPropertyRequest _request;
        private BaseResponse<EditPropertyResponse> _res;
        private EditPropertyResponse _response;
        private string _apiCode = "EditProperty";
        private TbProperty _Property;
        public EditPropertyController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditPropertyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditPropertyResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.Message = "Chỉnh sửa thành công !!!";
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
                _Property = _context.TbProperties.Where(p => p.Id == _request.Id /*&& p.InActive == false*/).FirstOrDefault();
                if (_Property != null)
                {
                    _Property.Name = _request.Name != null ? _request.Name : _Property.Name;
                    _Property.Active = _request.Active.HasValue ? _request.Active : _Property.Active;
                    _Property.UpdateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Property.UpdateDate = DateTime.Now; // Ngày hiện tại 
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
        public BaseResponse<EditPropertyResponse> Process(EditPropertyRequest request)
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
