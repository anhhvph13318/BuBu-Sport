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
using System.ComponentModel.DataAnnotations;

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
            var check = _context.TbCategories.Any(c => c.Id == _request.CategoryId);
            if (!check) {
                _context.TbCategories.Add(new TbCategory
                {
                    CreateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
                    CreateDate = DateTime.Now,
                    Id = _request.CategoryId,
                    Name = "Danh mục"
                });
            }
            
            _Produst = new TbProduct()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Code = _request.Code,
                Price = _request.Price,
                Quantity = _request.Quantity,
                Status = _request.Status,
                Brand = _request.Brand,
                Description = _request.Description,
                PriceNet = _request.PriceNet,
                ImageId = null,
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
                    _requestImage.Url = _request.UrlImage;
                    _requestImage.Type = "1";
                    _requestImage.ProductId = _Produst.Id;
                    var id = new CreateImageController(_context).Process(_requestImage);
                    _Produst.ImageId = id.Data.ID;
                #endregion
            }
        }

        public void PreValidation()
        {
            var errors = new List<string>();
            if( string.IsNullOrWhiteSpace(_request.Name) )
            {
                errors.Add("Tên sản phẩm không được để trống");
            }    
            if(_request.Price < 0)
            {
                errors.Add("Giá sản phẩm không được < 0");
            }    
            if(_request.PriceNet < 0)
            {
                errors.Add("Giá bán không được < 0 ");

            }   
            if(string.IsNullOrWhiteSpace(_request.Brand))
            {
                errors.Add("Không được để trống thương hiệu");
            }
            if(_request.Quantity < 0)
            {
                errors.Add("Số lượng không được < 0");
            }    
            if(_request.CategoryId == Guid.Empty)
            {
                errors.Add("Bạn chưa chọn danh mục");
            }    
            if(errors.Count > 0 )
            {
                throw new ValidationException(string.Join("; ", errors));
            }    
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateProductResponse> Process(CreateProductRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                PreValidation();
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
