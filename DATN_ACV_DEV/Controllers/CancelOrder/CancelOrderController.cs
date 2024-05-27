using AutoMapper;
using Azure.Core;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.CancelOrder_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.CancelOrder
{
    [Route("api/CancelOrder")]
    [ApiController]
    public class CancelOrderController : ControllerBase, IBaseController<CancelOrderRequest, CancelOrderResponse>
    {
        private readonly DBContext _context;

        private CancelOrderRequest _request;
        private BaseResponse<CancelOrderResponse> _res;
        private CreateImageRequest _requestImage = new CreateImageRequest();
        private CancelOrderResponse _response;
        private TbOrder _Order;
        public CancelOrderController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CancelOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CancelOrderResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.Message = "Gửi yêu cầu thành công";
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
                _Order = _context.TbOrders.Where(c => c.Id == _request.Id).FirstOrDefault();
                if (_Order != null)
                {
                    _Order.ReasionCancel = _request.ReasonCancel;
                    _Order.UpdateDate = DateTime.Now;
                }
                if (_request.UrlImage.Count > 1)
                {
                    #region Lưu ảnh danh mục
                    foreach (var item in _request.UrlImage)
                    {
                        _requestImage.Url = item;
                        _requestImage.Type = "3";
                        _requestImage.ProductId = _Order.Id;
                        var id = new CreateImageController(_context).Process(_requestImage);
                        //_Order.ImageId = id.Data.ID;
                    }
                    #endregion
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
        public BaseResponse<CancelOrderResponse> Process(CancelOrderRequest request)
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
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
