using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/DetailProduct")]
    [ApiController]
    public class DetailProductController : ControllerBase, IBaseController<DetailProductRequest, DetailProductResponse>
    {
        private readonly DBContext _context;
        private DetailProductRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<DetailProductResponse> _res;
        private DetailProductResponse _response;
        private string _apiCode = "DetailProduct";
        private TbProduct _Product;
        public DetailProductController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<DetailProductResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Product = new TbProduct();
            _response = new DetailProductResponse();
        }
        public void AccessDatabase()
        {
            try
            {
                _Product = _context.TbProducts.Where(p => p.Id == _request.ID && p.IsDelete == false).FirstOrDefault();
                var PrivateAtrtibute = _context.TbProperties.Where(c => c.ProductId == _request.ID).ToList();
                var PropertiesName = _context.TbProperties.Where(a => PrivateAtrtibute.Select(x => x.Id).Contains(a.Id) && a.Active != false).Select(n => n.Id).ToList();
                var Image = _context.TbImages.Where(i => i.ProductId == _Product.Id).Select(c=>c.Url).ToList();
                if (_Product != null)
                {
                    _response.Id = _Product.Id;
                    _response.Name = _Product.Name;
                    _response.Code = _Product.Code;
                    _response.Price = _Product.Price;
                    _response.Quantity = _Product.Quantity;
                    _response.Status = _Product.Status;
                    _response.Description = _Product.Description;
                    _response.PriceNet = _Product.PriceNet;
                    _response.Vat = _Product.Vat;
                    _response.Warranty = _Product.Warranty;
                    _response.Color = _Product.Color;
                    _response.Material = _Product.Material;
                    _response.Image = Image.FirstOrDefault();
                    //_response.CategoryName = _Product.Category.Name;
                    _response.PropertyID = PropertiesName;
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
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
        public BaseResponse<DetailProductResponse> Process(DetailProductRequest request)
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
