using DATN_ACV_DEV.FileBase;
using Microsoft.AspNetCore.Mvc;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Entity;
using Azure;
using Microsoft.EntityFrameworkCore;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using System.Net.WebSockets;

namespace DATN_ACV_DEV.Controllers.ProductDetail
{
    [Route("api/CreateProductDetail")]
    [ApiController]
    public class CreateProductDetailController : ControllerBase, IBaseController<CreateProductDetailRequest, CreateProductDetailResponse>
    {
        private readonly DBContext _context;
        private CreateProductDetailRequest _request;
        private BaseResponse<CreateProductDetailResponse> _res;
        private CreateProductDetailResponse _response;
        private string _apiCode = "CreateProductDetail";
        private TbProductDetail _ProductDetail;
        private TbProduct _Product;
        private List<TbProductDetail> _ProductDetails;
        private CreateImageRequest _Image; 
        private BaseResponse<CreateImageResponse> _imageId;
        public CreateProductDetailController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateProductDetailResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateProductDetailResponse();
        }
        public void AccessDatabase()
        {
            if (_request.SizesQuantities != null)
            {
                _context.TbProductDetails.AddRange(_ProductDetails); // Lưu nhiều bản ghi cùng lúc
                _context.SaveChanges();
                _Product = _context.TbProducts.Where(p => p.Id == _request.ProductID).FirstOrDefault();
                var quantityProduct = _context.TbProductDetails.Where(p => p.ProductId == _Product.Id).Sum(p => p.Quantity);
                _Product.Quantity = _Product.Quantity != null ? quantityProduct : 0;
                _context.SaveChanges(); // Lưu thay đổi số lượng              
            }
            else 
            {
                _context.TbProductDetails.AddRange(_ProductDetails); // Lưu nhiều bản ghi cùng lúc
                _context.SaveChanges();
            }
            _response.ID = _ProductDetails.First().Id;
            _res.Data = _response;
            //_context.Add(_Image);
            //_context.SaveChanges();
        }

        public void CheckAuthorization()
        {
            //_request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            var imageID = _context.TbProducts
                                  .Where(c => c.Id == _request.ProductID)
                                  .Select(c => c.ImageId)
                                  .FirstOrDefault();

            _ProductDetails = new List<TbProductDetail>(); // Danh sách ProductDetail
            // Nếu không có ImageID, tạo ảnh mới
            if (_request.ImageID == null)
            {
                _Image = new CreateImageRequest()
                {
                    Url = _request.UrlImage,
                    Type = "1",
                    InAcitve = true,
                    ProductId = _request.ProductID,
                };
                _imageId = new CreateImageController(_context).Process(_Image); // Nhận response
            }
            // Kiểm tra nếu SizesQuantities không null và có dữ liệu
            if (_request.SizesQuantities != null && _request.SizesQuantities.Any())
            {
                foreach (var sizeQuantity in _request.SizesQuantities)
                {
                    var productDetail = new TbProductDetail()
                    {
                        Id = Guid.NewGuid(),
                        Price = _context.TbProducts.Where(c=>c.Id == _request.ProductID).Select(c=>c.Price).FirstOrDefault(),
                        Quantity = sizeQuantity.QuantitySize,
                        ImageId = _request.ImageID ?? _imageId.Data.ID,
                        ColorId = _request.Color,
                        SizeId = sizeQuantity.IdSize,
                        ProductId = _request.ProductID,
                        CreateDate = DateTime.Now,
                    };
                    _ProductDetails.Add(productDetail);
                }
            }
            else // Nếu không có SizesQuantities, vẫn tạo ít nhất một bản ghi
            {
                var productDetail = new TbProductDetail()
                {
                    Id = Guid.NewGuid(),
                    Price = _request.Price,
                    Quantity = _request.Quantity, // Lấy Quantity từ request nếu không có danh sách
                    ImageId = _request.ImageID ?? _imageId.Data.ID,
                    ColorId = _request.Color,
                    SizeId = _request.SizeName, // Trường hợp không có danh sách, lấy SizeName trực tiếp từ request
                    ProductId = _request.ProductID,
                    CreateDate = DateTime.Now,
                };
                _ProductDetails.Add(productDetail);
            }

            
        }



        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateProductDetailResponse> Process(CreateProductDetailRequest request)
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
