using Azure;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Model_DTO.SendEmail_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Order
{

    [Route("api/UpdateStatusONOrder")]
    [ApiController]
    public class UpdateStatusONOrderController : ControllerBase, IBaseController<UpdatStatusOrderRequest, UpdatStatusOrderResponse>
    {
        private readonly DBContext _context;
        private UpdatStatusOrderRequest _request;
        private BaseResponse<UpdatStatusOrderResponse> _res;
        private UpdatStatusOrderResponse _response;
        private string _apiCode = "UpdateStatusONOrder";
        private TbOrder _Order;
        public UpdateStatusONOrderController(DBContext context) 
        {
            _context = context;
            _res = new BaseResponse<UpdatStatusOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new UpdatStatusOrderResponse();
        }
        public void AccessDatabase()
        {
            _res.Data = _response;
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            TbProductDetail tbProductDetail = new TbProductDetail();
            _Order = _context.TbOrders.Where(c => c.Id == _request.id).FirstOrDefault();
            if (_request.products != null)
            {
                foreach (var item in _request.products)
                {
                    tbProductDetail = _context.TbProductDetails.Where(c => c.Id == item.Id).FirstOrDefault();
                    if (tbProductDetail.Quantity > 0)
                    {
                        tbProductDetail.Quantity -= item.Quantity;
                    }
                }
            }
            if (_Order != null)
            {
                _Order.Status = _request.status;
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<UpdatStatusOrderResponse> Process(UpdatStatusOrderRequest request)
        {
            try
            {
                _request = request;
                GenerateObjects();
                AccessDatabase();
            }
            catch (Exception)
            {

                throw;
            }
            return _res;
        }
    }
}
