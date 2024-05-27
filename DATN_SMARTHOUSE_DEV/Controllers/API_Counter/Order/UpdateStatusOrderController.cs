using AutoMapper;
using Azure;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DATN_ACV_DEV.Controllers.API_Counter.Order
{
    [Route("api/UpdateStatusOrder")]
    [ApiController]
    public class UpdateStatusOrderController : ControllerBase, IBaseController<UpdatStatusOrderRequest, UpdatStatusOrderResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private UpdatStatusOrderRequest _request;
        private BaseResponse<UpdatStatusOrderResponse> _res;
        private UpdatStatusOrderResponse _response;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "id";
        private string _conC02Field = "status";
        private string _apiCode = "UpdateStatusOrder";
        public UpdateStatusOrderController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<UpdatStatusOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new UpdatStatusOrderResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            var model = _context.TbOrders.Where(o => o.Id == _request.id).FirstOrDefault();
            if (model != null)
            {
                model.Status = _request.status;
                if (model.Status == Utility.Utility.ORDER_STATUS_RETURNS_PRODUCT)
                {
                    var orderDetail = _context.TbOrderDetails.Where(c => c.OrderId == model.Id);
                    orderDetail.ToList().ForEach(p =>
                    {
                        var product = _context.TbProducts.Where(c => c.Id == p.ProductId).FirstOrDefault();
                        product.Quantity += p.Quantity;
                    });
                }
                // nếu đơn hàng được mua tại quầy khi cập nhật trạng thái là hủy đơn số lượng sản phẩm sẽ được hoàn về kho.
                if (model.Status == Utility.Utility.ORDER_STATUS_CANCELLED && model.OrderCounter == true)
                {
                    var orderDetail = _context.TbOrderDetails.Where(c => c.OrderId == model.Id);
                    orderDetail.ToList().ForEach(p =>
                    {
                        var product = _context.TbProducts.Where(c => c.Id == p.ProductId).FirstOrDefault();
                        product.Quantity += p.Quantity;
                    });
                }
                _response.id = model.Id;
            }
            _context.SaveChanges();
            _res.Data = _response;
        }
        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);

        }
        public void GenerateObjects()
        {
        }

        public void PreValidation()
        {
            // Đơn hàng không tồn tại
            Condition.ConditionOrder.UpdateStatusOrder_C01(_context, _request, _apiCode, _conC01, _conC01Field);
            // Trạng thái Đơn hàng không hợp lệ
            Condition.ConditionOrder.UpdateStatusOrder_C02(_context, _request, _apiCode, _conC02, _conC02Field);
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<UpdatStatusOrderResponse> Process(UpdatStatusOrderRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
                //GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex0)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex0.Messages;
            }
            catch (Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;

        }
    }
}
