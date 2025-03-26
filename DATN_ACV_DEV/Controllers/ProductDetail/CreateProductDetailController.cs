using DATN_ACV_DEV.FileBase;
using Microsoft.AspNetCore.Mvc;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Entity;
using Azure;
using Microsoft.EntityFrameworkCore;
using DATN_ACV_DEV.Controllers.Property;
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
        private CreateImageRequest _Image;
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
            _context.Add(_ProductDetail);
            _context.SaveChanges();
            _response.ID = _ProductDetail.Id;
            _res.Data = _response;
            _context.Add(_Image);
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {
            //_request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            var imageID = _context.TbProducts.Where(c => c.Id == _request.ProductID).Select(c => c.ImageId).FirstOrDefault();
            _ProductDetail = new TbProductDetail()
            {
                Id = Guid.NewGuid(),
                Price = _request.Price,
                Quantity = _request.Quantity,
                ImageId = imageID,
                ColorId = _request.Color,
                SizeId = _request.SizeName,
                ProductId = _request.ProductID,
            };
            _Image = new CreateImageRequest()
            {
                Url = _request.UrlImage,
                Type = "1",
                InAcitve = true,
                ProductId = _request.ProductID,
            
            };
            var id = new CreateImageController(_context).Process(_Image);
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
