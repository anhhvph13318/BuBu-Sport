using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/DetailCategory")]
    [ApiController]
    public class DetailCategoryController : ControllerBase, IBaseController<DetailCategoryRequest, DetailCategoryResponse>
    {
        private readonly DBContext _context;
        private DetailCategoryRequest _request;
        private BaseResponse<DetailCategoryResponse> _res;
        private DetailCategoryResponse _response;
        private string _apiCode = "DetailCategory";
        private TbCategory _Category;
        public DetailCategoryController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailCategoryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DetailCategoryResponse();
        }

        public void AccessDatabase()
        {
            _Category = _context.TbCategories.Where(c => c.Id == _request.ID && c.IsDelete == false).FirstOrDefault();
            var Image = _context.TbImages.Where(i => i.Id == _Category.ImageId).FirstOrDefault();
            if (_Category != null)
            {
                _response.category.Id = _Category.Id;
                _response.category.Name = _Category.Name;
                _response.category.Description = _Category.Description;
                _response.category.Status = _Category.Status == 1 ? Utility.Utility.Status_Category_Active : Utility.Utility.Status_Category_No_Active;
                _response.category.Image = Image != null ? Image.Url : "không có ảnh";
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
        public BaseResponse<DetailCategoryResponse> Process(DetailCategoryRequest request)
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
