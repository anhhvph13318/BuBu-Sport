using AutoMapper;
using DATN_ACV_DEV.Controllers.Property;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/EditProduct")]
    [ApiController]
    public class EditProductController : ControllerBase, IBaseController<EditProductRequest, EditProductResponse>
    {
        private readonly DBContext _context;
        private EditProductRequest _request;
        private BaseResponse<EditProductResponse> _res;
        private EditPropertyRequest _requestEditProperty = new EditPropertyRequest();
        private CreatePropertyRequest _requestProperty = new CreatePropertyRequest();
        private EditProductResponse _response;
        private string _apiCode = "EditProduct";
        private TbProduct _Produst;
        private TbImage _Image;
        public EditProductController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditProductResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditProductResponse();
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
                _Produst = _context.TbProducts.Where(p => p.Id == _request.ID && p.IsDelete == false).FirstOrDefault();
                _Image = _context.TbImages.Where(c => c.ProductId == _Produst.Id && c.Type == "1").FirstOrDefault();
                if (_Produst != null)
                {
                    _Produst.Name = _request.Name != null ? _request.Name : _Produst.Name;
                    _Produst.Code = _request.Code != null ? _request.Code : _Produst.Code;
                    _Produst.Price = _request.Price != null ? _request.Price : _Produst.Price;
                    _Produst.Status = _request.Status != null ? _request.Status : _Produst.Status;
                    _Produst.Description = _request.Description != null ? _request.Description : _Produst.Description ;
                    _Produst.PriceNet = _request.PriceNet != null ? _request.PriceNet : _Produst.PriceNet;
                    _Produst.ImageId = _request.ImageId != null ? _request.ImageId : _Produst.ImageId;
                    _Image.Url = _request.UrlImage.FirstOrDefault();
                    _Produst.CategoryId = (_request.CategoryId != null && _request.CategoryId != new Guid()) ? _request.CategoryId : _Produst.CategoryId;
                    _Produst.UpdateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Produst.UpdateDate = DateTime.Now; // Ngày hiện tại 
                }
                if (_request.PropertyID != null && _request.TypeEditProperty == "1")
                {
                    foreach (var item in _request.PropertyID)
                    {
                        var nameproperty = _context.TbProperties.Where(c => c.Id == item).Select(c => c.Name).FirstOrDefault();
                        _requestEditProperty.Active = false;
                        _requestEditProperty.Name = nameproperty;
                        _requestEditProperty.Id = item;
                        new EditPropertyController(_context).Process(_requestEditProperty);
                    }
                }
                if (_request.PropertyID != null && _request.TypeEditProperty == "0")
                {
                    foreach (var item in _request.PropertyID)
                    {
                        var nameproperty = _context.TbProperties.Where(c => c.Id == item).Select(c => c.Name).FirstOrDefault();
                        _requestProperty.Name = nameproperty;
                        _requestProperty.ProductId = _request.ID;
                        _requestProperty.CategoryId = _request.CategoryId;
                        new CreatePropertyController(_context).Process(_requestProperty);
                    }
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }

        }

        public void PreValidation()
        {
            // Code không được trùng nhau
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditProductResponse> Process(EditProductRequest request)
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
