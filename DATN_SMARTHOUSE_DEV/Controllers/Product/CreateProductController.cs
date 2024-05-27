using AutoMapper;
using Azure.Core;
using DATN_ACV_DEV.Controllers.Property;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CreateProduct")]
    [ApiController]
    public class CreateProductController : ControllerBase, IBaseController<CreateProductRequest, CreateProductResponse>
    {
        private readonly DBContext _context;
        private CreateProductRequest _request;
        private BaseResponse<CreateProductResponse> _res;
        private CreateProductResponse _response;
        private CreateImageRequest _requestImage = new CreateImageRequest();
        private CreatePropertyRequest _requestProperty = new CreatePropertyRequest();
        private string _apiCode = "CreateProduct";
        private TbProduct _Produst;
        private TbProperty _Property;
        public CreateProductController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateProductResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateProductResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Produst);
            _context.SaveChanges();
            _response.ID = _Produst.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            _Produst = new TbProduct()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Code = _request.Code,
                Price = _request.Price,
                Quantity = 0,
                Status = _request.Status,
                Description = _request.Description,
                PriceNet = _request.PriceNet,
                ImageId = _request.ImageId,
                CategoryId = _request.CategoryId,
                Vat = _request.Vat,
                Warranty = _request.Warranty,
                Color = _request.Color,
                Material = _request.Material,
                //Default
                CreateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"), // Tạm thời gán guid khởi tạo.
                CreateDate = DateTime.Now, // Ngày hiện tại 
                IsDelete = false,
            };
            if (_request.PropertyID != null) // trường hợp đã có thuộc tính trong danh mục trước đó
            {
                #region Lưu thuộc tính sản phẩm
                foreach (var item in _request.PropertyID)
                {
                    _requestProperty.Name = _context.TbProperties.Where(c => c.Id == item).Select(c => c.Name).FirstOrDefault();
                    _requestProperty.ProductId = _Produst.Id;
                    _requestProperty.CategoryId = _Produst.CategoryId;
                    var id = new CreatePropertyController(_context).Process(_requestProperty);
                }
                #endregion
            }
            if (_request.UrlImage != null)
            {
                #region Lưu ảnh sản phẩm
                foreach (var item in _request.UrlImage)
                {
                    _requestImage.Url = item;
                    _requestImage.Type = "1";
                    _requestImage.ProductId = _Produst.Id;
                    var id = new CreateImageController(_context).Process(_requestImage);
                    _Produst.ImageId = id.Data.ID;
                }
                #endregion
            }
        }

        public void PreValidation()
        {
            // Code không được trùng nhau
            // Bắt buộc chọn sản sản phẩm
            // Bắt buộc chọn danh mục sản phẩm
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateProductResponse> Process(CreateProductRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
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
