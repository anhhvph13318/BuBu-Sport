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
    [Route("api/EditCategory")]
    [ApiController]
    public class EditCategoryController : ControllerBase, IBaseController<EditCategoryRequest, EditCategoryResponse>
    {
        private readonly DBContext _context;
        private EditCategoryRequest _request;
        private BaseResponse<EditCategoryResponse> _res;
        private EditCategoryResponse _response;
        private string _apiCode = "EditCategory";
        private TbCategory _Category;
        private TbImage _Image;
        public EditCategoryController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditCategoryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditCategoryResponse();
        }

        public void AccessDatabase()
        {
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
            try
            {
                _Category = _context.TbCategories.Where(c => c.Id == _request.ID && c.IsDelete == false).FirstOrDefault();
                _Image = _context.TbImages.Where(c => c.ProductId == _Category.Id && c.Type == "2").FirstOrDefault();
                if (_Category != null)
                {
                    _Category.Name = _request.Name != null ? _request.Name : _Category.Name;
                    _Category.Description = _request.Description != null ? _request.Description : _Category.Description;
                    _Category.ImageId = _request.ImageId != null ? _request.ImageId : _Category.ImageId;
                    _Category.Status = _request.Status != null ? _request.Status : _Category.Status;
                    _Category.Description = _request.Description != null ? _request.Description : _Category.Description;
                    _Image.Url = _request.UrlImage.FirstOrDefault();
                    _Category.UpdateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Category.UpdateDate = DateTime.Now; // Ngày hiện tại 
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
        public BaseResponse<EditCategoryResponse> Process(EditCategoryRequest request)
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
