using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CreateCategory")]
    [ApiController]
    public class CreateCategoryController : ControllerBase, IBaseController<CreateCategoryRequest, CreateCategoryResponse>
    {
        private readonly DBContext _context;
        private CreateCategoryRequest _request;
        private BaseResponse<CreateCategoryResponse> _res;
        private CreateImageRequest _requestImage = new CreateImageRequest();
        private CreateCategoryResponse _response;
        private string _apiCode = "CreateCategory";
        private TbCategory _Category;
        public CreateCategoryController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateCategoryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateCategoryResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Category);
            _context.SaveChanges();
            _response.ID = _Category.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Category = new TbCategory()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Status = _request.Status,
                ImageId = _request.ImageId,
                Description = _request.Description,
                //Default
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"), // Tạm thời gán guid khởi tạo.
                CreateDate = DateTime.Now, // Ngày hiện tại 
                IsDelete = false,
            };
            if (_request.UrlImage != null)
            {
                foreach (var item in _request.UrlImage)
                {
                    #region Lưu ảnh danh mục
                    _requestImage.Url = item;
                    _requestImage.Type = "2";
                    _requestImage.ProductId = _Category.Id;
                    var id = new CreateImageController(_context).Process(_requestImage);
                    _Category.ImageId = id.Data.ID;
                    #endregion
                }

            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateCategoryResponse> Process(CreateCategoryRequest request)
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
