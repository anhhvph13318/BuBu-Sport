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

namespace DATN_ACV_DEV.Controllers.API_Counter.Order
{
    [Route("api/ConfirmOrderCounter")]
    [ApiController]
    public class ConfirmOrderCounterController : ControllerBase, IBaseController<ConfirmOrderCounterRequest, ConfirmOrderCounterResponse>
    {
        private readonly DBContext _context;
        private ConfirmOrderCounterRequest _request;
        private BaseResponse<ConfirmOrderCounterResponse> _res;
        private ConfirmOrderCounterResponse _response;
        private string _apiCode = "ConfirmOrderCounter";
        private TbOrder _order;
        private TbAccount account;
        private TbCustomer customer;
        private TbOrderDetail _orderDetail;
        private List<OrderProduct> _listProduct;
        private List<TbOrderDetail> _lstOrderDetail;
        private List<TbCartDetail> _listCartDetail;
        private List<string> _lstVoucherCode;
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
        public ConfirmOrderCounterController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<ConfirmOrderCounterResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new ConfirmOrderCounterResponse();
            _listProduct = new List<OrderProduct>();
            _lstOrderDetail = new List<TbOrderDetail>();
            _listCartDetail = new List<TbCartDetail>();
            _lstVoucherCode = new List<string>();
        }

        public void AccessDatabase()
        {
            _context.Add(_order);
            _context.AddRange(_lstOrderDetail);
            _context.RemoveRange(_listCartDetail);

            _response.id = _order.Id;
            _response.orderCode = _order.OrderCode;
            _res.Data = _response;
            if (_request.voucherID != null)
            {
                var voucher = _context.TbVouchers.Where(v => v.Code == _request.voucherCode).FirstOrDefault();
                if (voucher != null)
                {
                    voucher.Quantity -= 1;
                }
            }
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);

        }

        public void GenerateObjects()
        {
            _order = new TbOrder()
            {
                Id = Guid.NewGuid(),
                OrderCode = "ACV_" + DateTime.Now.Millisecond,
                TotalAmount = _request.totalAmount,
                TotalAmountDiscount = _request.totalAmountDiscount,
                Description = _request.description,
                AccountId = _request.UserId,
                PaymentMethodId = _request.paymentMethodId,
                VoucherCode = _request.voucherCode,
                AmountShip = 0,
                OrderCounter = true,// hóa đơn bán tại quầy. true: bán tại quầy, false or null bán online
                PhoneNumberCustomer = _request.phoneNumber,
                //Defautl
                CreateBy = _request.AdminId ?? Guid.NewGuid(),
                CreateDate = DateTime.Now,
                Status = Utility.Utility.ORDER_DURING_THE_RETURN_PERIOD
            };
            foreach (var i in _listProduct)
            {
                _orderDetail = new TbOrderDetail()
                {
                    Id = Guid.NewGuid(),
                    OrderId = _order.Id,
                    ProductId = i.productId,
                    Quantity = i.quantity
                };
                //giảm số lượng sản phẩm khi mua tại quầy ngay khi tạo hóa đơn
                var product = _context.TbProducts.Where(c => c.Id == i.productId).FirstOrDefault();
                product.Quantity -= i.quantity;
                //đến đây
                _lstOrderDetail.Add(_orderDetail);
            }
        }

        public void PreValidation()
        {
            #region Không chọn cart Detail và sản phẩm không tồn tại hoặc số lượng không đủ.
            ACV_Exception ACV_Exception;
            if (_request.cartDetailId != null)
            {
                var lstcartDetail = _context.TbCartDetails.Where(cartDetail => _request.cartDetailId.Contains(cartDetail.Id));
                _listCartDetail = lstcartDetail.ToList();
                foreach (var item in _listCartDetail)
                {
                    var model = _context.TbProducts.Where(product => product.Id == item.ProductId && product.IsDelete == false && product.Quantity >= item.Quantity).FirstOrDefault();
                    if (model != null)
                    {
                        var image = _context.TbImages.Where(i => i.Id == model.ImageId).FirstOrDefault();
                        OrderProduct product = new OrderProduct()
                        {
                            productId = model.Id,
                            categoryId = model.CategoryId,
                            productName = model.Name,
                            productCode = model.Code,
                            price = model.Price,
                            weight = model.Weight,
                            quantity = item.Quantity.Value,
                            url = image != null ? image.Url : ""
                        };
                        _listProduct.Add(product);
                    }
                    if (model == null)
                    {
                        ACV_Exception = new ACV_Exception();
                        //To-do: Lay thong message text tu message code
                        ACV_Exception.Messages.Add(Message.CreateErrorMessage(_apiCode, _conC02, "Sản phẩm có mã :" + model.Code + Utility.Utility.PRODUCT_NOTFOUND, _conC02Field));
                        throw ACV_Exception;
                    }
                }
            }
            else
            {
                ACV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                ACV_Exception.Messages.Add(Message.CreateErrorMessage(_apiCode, _conC01, Utility.Utility.ORDER_CARTDETAIL_NOTFOUND, _conC01Field));
                throw ACV_Exception;
            }
            #endregion

            if (_request.voucherID != null)
            {
                #region voucher không tồn tại or số lượng không đủ or voucher type không phải là discount
                ConditionOrder.CreateOrderCouter_C03(_context, _request.voucherCode, _apiCode, _conC04, _conC04Field);
                #endregion      
            }
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<ConfirmOrderCounterResponse> Process(ConfirmOrderCounterRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
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
