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
using Microsoft.Identity.Client;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/ConfirmPayment")]
    [ApiController]
    public class ConfirmPaymentController : ControllerBase, IBaseController<OrderStatusRequest, OrderStatusResponse>
    {
        private readonly DBContext _context;
        private OrderStatusRequest _request;
        private BaseResponse<OrderStatusResponse> _res;
        private OrderStatusResponse _response;
        private string _apiCode = "ConfirmPayment";
        private TbOrder _order;
        private TbAccount account;
        private TbCustomer customer;
        private TbOrderDetail _orderDetail;
        private List<OrderProduct> _listProduct;
        private List<TbOrderDetail> _lstOrderDetail;
        private List<TbCartDetail> _listCartDetail;
        private List<string> _lstVoucherCode;
        private bool AddCustomer = false;
        private decimal? _totalDiscount = 0;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC03 = "C03";
        private string _conC04 = "C04";
        private string _conC01Field = "cartDetailID";
        private string _conC02Field = "ProductID";
        private string _conC03Field = "VoucherID";
        private string _conC04Field = "VoucherID";
        private string _namePaymentMethod;
        public ConfirmPaymentController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<OrderStatusResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new OrderStatusResponse();
            _listProduct = new List<OrderProduct>();
            _lstOrderDetail = new List<TbOrderDetail>();
            _listCartDetail = new List<TbCartDetail>();
            _lstVoucherCode = new List<string>();
        }

        public void AccessDatabase()
        {
            _context.Update(_order);

			_response.orderId = _request.orderId;
            _response.paymentMethod = _request.paymentMethod;
            _response.paymentStatus = _request.paymentStatus;
            _res.Data = _response;
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {

            _request.AuthorizationCustomer(_context, _apiCode);
        }

        public void GenerateObjects()
        {
			ACV_Exception ACV_Exception;
            _order = _context.TbOrders.FirstOrDefault(c => (_request.orderId != Guid.Empty && c.Id == _request.orderId) || (_request.orderId == Guid.Empty && _request.orderCode == c.OrderCode));
			if (_order == null) {
				ACV_Exception = new ACV_Exception();
				ACV_Exception.Messages.Add(Message.CreateErrorMessage(_apiCode, _conC01, "Đơn hàng có mã :" + _request.orderId + Utility.Utility.ORDER_NOTFOUND, _conC01Field));
                throw ACV_Exception;
            }
            else
            {
                _order.PaymentMethod = _request.paymentMethod;
                _order.PaymentStatus = (short)_request.paymentStatus;
            }
        }

        public void PreValidation()
        {

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<OrderStatusResponse> Process([FromBody]OrderStatusRequest request)
        {
            try
            {
				//request.UserId = new Guid("65809962-D69A-4D1F-9C14-E4D28DA106C4");
				_request = request;

                //CheckAuthorization();
                //PreValidation();
                GenerateObjects();
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
