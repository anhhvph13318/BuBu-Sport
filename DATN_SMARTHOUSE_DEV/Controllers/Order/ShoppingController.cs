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
    [Route("api/Shopping")]
    [ApiController]
    public class ShoppingController : ControllerBase, IBaseController<ShoppingRequest, ShoppingResponse>
    {
        private readonly DBContext _context;
        private ShoppingRequest _request;
        private BaseResponse<ShoppingResponse> _res;
        private ShoppingResponse _response;
        private string _apiCode = "Shopping";
        //private TbOrder _order;
        private TbAccount account;
        private TbCustomer customer;
        private TbOrder _order;
        private TbOrderDetail _orderDetail;
        private List<TbOrderDetail> _lstOrderDetail;
        private List<OrderProduct> _listProduct;
        private List<TbCartDetail> _listCartDetail;
        private List<string> _lstVoucherCode;
        private List<Guid> _lstVoucherId;
        private decimal? _totalDiscount = 0;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC03 = "C03";
        private string _conC04 = "C04";
        private string _conC01Field = "cartDetailID";
        private string _conC02Field = "ProductID";
        private string _conC03Field = "VoucherID";
        private string _conC04Field = "VoucherID";
        private decimal? _amountShip = 0;
        private decimal? _totalAmount = 0;
        private bool statusVoucherShip = true;
        public ShoppingController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<ShoppingResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new ShoppingResponse();
            _listProduct = new List<OrderProduct>();
            _lstOrderDetail = new List<TbOrderDetail>();
            _listCartDetail = new List<TbCartDetail>();
            _lstVoucherCode = new List<string>();
            _lstVoucherId = new List<Guid>();
        }

        public void AccessDatabase()
        {
            var product = _context.TbProducts.Where(c => c.Id == _request.productId && c.IsDelete == false).FirstOrDefault();
            if (product != null)
            {
                _order = new TbOrder
                {
                    Id = Guid.NewGuid(),
                    OrderCode = "ACV_" + DateTime.Now.Millisecond,
                    AmountShip = _request.amountShip,
                    TotalAmount = (product.Price * _request.quantity),
                    CreateDate = DateTime.Now,
                    Status = Utility.Utility.ORDER_STATUS_PREPARE_GOODS,
                    PaymentMethodId = _request.paymentMethodId,
                    OrderCounter = false,
                    PhoneNumberCustomer = _request.phoneNumber,
                    AddressCustomer = _request.address,
                };
                _orderDetail = new TbOrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    OrderId = _order.Id,
                    Quantity = _request.quantity
                };
                _context.Add(_order);
                _context.Add(_orderDetail);
                _context.SaveChanges();
                _response.orderCode = _order.OrderCode;
                _response.id = _order.Id;
                _response.amountShip = _request.amountShip;
                _response.totalAmount = _order.TotalAmount;
                _response.product = new OrderProduct
                {
                    productId = product.Id,
                    categoryId = product.CategoryId,
                    productName = product.Name,
                    productCode = product.Code,
                    price = product.Price,
                    quantity = _orderDetail.Quantity,
                };
            }
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
        }

        public void GenerateObjects()
        {

        }

        public void PreValidation()
        {

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<ShoppingResponse> Process(ShoppingRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                // PreValidation();
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
