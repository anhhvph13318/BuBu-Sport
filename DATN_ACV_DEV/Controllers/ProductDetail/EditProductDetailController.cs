using Azure;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.ProductDetail
{
    [Route("api/EditProductDetail")]
    [ApiController]
    public class EditProductDetailController : ControllerBase, IBaseController<CreateProductDetailRequest, CreateProductDetailResponse>
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
        public EditProductDetailController(DBContext context)
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
            _response.ID = (Guid)_ProductDetail.ProductId;
            _res.Data = _response;
            _context.SaveChanges();
            _Product = _context.TbProducts.Where(p => p.Id == _ProductDetail.ProductId).FirstOrDefault();
            var quantityProduct = _context.TbProductDetails.Where(c => c.ProductId == _Product.Id).Sum(c => c.Quantity);
            _Product.Quantity = _Product.Quantity != null ? quantityProduct : 0;
            _context.SaveChanges(); // Lưu thay đổi số lượng              

        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            if (_request != null)
            {
                if (_request.SizesQuantities != null && _request.SizesQuantities.Any())
                {
                    _ProductDetail = _context.TbProductDetails.Where(c => c.Id == _request.ProductID).FirstOrDefault();
                    var imageID = _context.TbImages.Where(c => c.Id == _ProductDetail.ImageId).FirstOrDefault();
                    imageID.Url = _request.UrlImage != null ? _request.UrlImage : null;
                    if (_ProductDetail != null)
                    {
                        _ProductDetail.Quantity = _request.SizesQuantities.First().QuantitySize;
                        _ProductDetail.ImageId = imageID.Id;
                        _ProductDetail.ColorId = _request.Color;
                        _ProductDetail.SizeId = _request.SizesQuantities.First().IdSize;
                    }
                }
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
