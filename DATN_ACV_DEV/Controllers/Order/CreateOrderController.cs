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
    [Route("api/CreateOrder")]
    [ApiController]
    public class CreateOrderController : ControllerBase, IBaseController<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly DBContext _context;
        private CreateOrderRequest _request;
        private BaseResponse<CreateOrderResponse> _res;
        private CreateOrderResponse _response;
        private string _apiCode = "CreateOrder";
        //private TbOrder _order;
        private TbAccount account;
        private TbCustomer customer;
        private TbAddressDelivery addressDelivery;
        private TbOrderDetail _orderDetail;
        private List<TbOrderDetail> _lstOrderDetail;
        private List<OrderProduct> _listProduct;
        private List<TbCartDetail> _listCartDetail;
        private List<string> _lstVoucherCode;
        private List<Guid> _lstVoucherId;
        private IQueryable<TbVoucher> _lstVoucher;
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
        public CreateOrderController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateOrderResponse();
            _listProduct = new List<OrderProduct>();
            _lstOrderDetail = new List<TbOrderDetail>();
            _listCartDetail = new List<TbCartDetail>();
            _lstVoucherCode = new List<string>();
            _lstVoucherId = new List<Guid>();
        }

        public void AccessDatabase()
        {
            _response.amountShip = _amountShip;
            _response.products = _listProduct;
            _response.totalAmount = _totalAmount;
            _response.totalAmountDiscount = _totalDiscount;
            _response.addressDelivery = addressDelivery.WardName + " " + addressDelivery.DistrictName + " " + addressDelivery.ProvinceName;
            _response.addressDeliveryId = addressDelivery.Id;
            _response.cartDetailId = _request.cartDetailID;
            _response.paymentMethodId = _request.paymentMenthodID;
            _response.VoucherCodes = _lstVoucherCode;
            _response.VoucherId = _lstVoucherId;
            _response.description = _request.description;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.AuthorizationCustomer(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            account = _context.TbAccounts.Where(a => a.Id == _request.UserId).FirstOrDefault();
            customer = _context.TbCustomers.Where(c => c.Id == account.CustomerId).FirstOrDefault();
            addressDelivery = _context.TbAddressDeliveries.Where(a => a.Id == _request.AddressDeliveryId && a.IsDelete == false).FirstOrDefault();
            GHNFeeRequest requesFee = new GHNFeeRequest()
            {
                service_type_id = Utility.Utility.SERVICE_TYPE_DEFAULT,
                insurance_value = Convert.ToInt32(_listProduct.Sum(p => p.price)),
                to_ward_code = addressDelivery.WardCode,
                to_district_id = addressDelivery.DistrictId,
                from_district_id = Utility.Utility.FORM_DISTRICT_ID_DEFAULT,
                weight = _listProduct.Sum(c => c.weight ?? 2000),
                //tạm thời fix cứng
                lenght = 20,
                width = 20,
                height = 20,
            };
            #region comment
            //_order = new TbOrder()
            //{
            //    Id = Guid.NewGuid(),
            //    OrderCode = "ACV_" + DateTime.Now.Millisecond,
            //    TotalAmount = 0,
            //    Description = _request.description,
            //    AccountId = _request.UserId,
            //    PaymentMethodId = _request.paymentMenthodID,
            //    VoucherId = _request.voucherID != null ? string.Join(",", _request.voucherID) : null,
            //    AmountShip = Common.GetFee(_request.TokenGHN, requesFee),
            //    CustomerId = customer.Id,
            //    //Defautl
            //    CreateBy = _request.UserId,
            //    CreateDate = DateTime.Now,
            //    Status = Utility.Utility.ORDER_STATUS_PREPARE_GOODS
            //};
            //foreach (var itemProduct in _listProduct)
            //{
            //    _orderDetail = new TbOrderDetail()
            //    {
            //        Id = Guid.NewGuid(),
            //        OrderId = _order.Id,
            //        ProductId = itemProduct.productId,
            //        Quantity = itemProduct.quantity
            //    };
            //    _lstOrderDetail.Add(_orderDetail);
            //    _response.products.Add(itemProduct);
            //    _order.TotalAmount += (itemProduct.price * itemProduct.quantity);
            //}
            #endregion
            //_amountShip = Common.GetFee(_request.TokenGHN, requesFee);
            if (_request.voucherID != null)
            {
                _lstVoucher = _context.TbVouchers.Where(voucher => _request.voucherID.Contains(voucher.Id));
            }
            foreach (var product in _listProduct)
            {
                if (_request.voucherID != null)
                {
                    var voucherDiscount = _lstVoucher.ToList().Where(c => c.Type == Utility.Utility.VOUCHER_DISCOUNT).FirstOrDefault();
                    if (voucherDiscount != null)
                    {

                        if (voucherDiscount.ProductId == product.productId)
                        {
                            var totalAmountPromotionApplyProduct = product.price * product.quantity;
                            var amountDiscount = Common.CalculateDiscount(totalAmountPromotionApplyProduct, voucherDiscount);
                            _totalAmount += (totalAmountPromotionApplyProduct - amountDiscount.DiscountVoucher);
                            _totalDiscount += amountDiscount.DiscountVoucher;
                            _lstVoucherCode.Add(voucherDiscount.Code);
                            _lstVoucherId.Add(voucherDiscount.Id);
                        }
                        else if (voucherDiscount.CategoryId == product.categoryId)
                        {
                            var totalAmountPromotionApplyCategory = product.price * product.quantity;
                            var amountDiscount = Common.CalculateDiscount(totalAmountPromotionApplyCategory, voucherDiscount);
                            _totalAmount += (totalAmountPromotionApplyCategory - amountDiscount.DiscountVoucher);
                            _totalDiscount = amountDiscount.DiscountVoucher;
                            _lstVoucherCode.Add(voucherDiscount.Code);
                            _lstVoucherId.Add(voucherDiscount.Id);
                        }
                        else
                        {
                            _totalAmount += (product.price * product.quantity);
                        }
                    }
                    else
                    {
                        _totalAmount += (product.price * product.quantity);
                    }
                }
                else
                {
                    _totalAmount += (product.price * product.quantity);
                }
            }
            // tính voucher free ship 
            if (_request.voucherID != null)
            {
                var vouchership = _lstVoucher.Where(c => c.Type == Utility.Utility.VOUCHER_FREESHIP).FirstOrDefault();
                if (vouchership != null)
                {
                    var amountDiscount = Common.CalculateDiscount(0, vouchership);
                    //_amountShip -= amountDiscount.DiscountShipping;
                    _lstVoucherCode.Add(vouchership.Code);
                    _lstVoucherId.Add(vouchership.Id);
                }
            }



        }

        public void PreValidation()
        {
            #region Không chọn cart Detail và sản phẩm không tồn tại hoặc số lượng không đủ.
            ACV_Exception ACV_Exception;
            if (_request.cartDetailID != null)
            {
                var lstcartDetail = _context.TbCartDetails.Where(cartDetail => _request.cartDetailID.Contains(cartDetail.Id));
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
                        ACV_Exception.Messages.Add(Message.CreateErrorMessage(_apiCode, _conC02, Utility.Utility.PRODUCT_NOTFOUND, _conC02Field));
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
                #region Voucher không tồn tại

                ConditionOrder.CreateOrder_C02(_context, _request.voucherID, _apiCode, _conC03, _conC03Field);

                #endregion

                #region Số lượng voucher lớn hơn 2
                ConditionOrder.CreateOrder_C03(_context, _request.voucherID, _apiCode, _conC04, _conC04Field);
                #endregion

                #region Type voucher giống nhau
                ConditionOrder.CreateOrder_C04(_context, _request.voucherID, _apiCode, _conC04, _conC04Field);
                #endregion
                #region Không có sản phẩm nào trong hóa đơn được áp dụng voucher.
                ConditionOrder.CreateOrder_C06(_context, _request, _apiCode, _conC04, _conC04Field);
                #endregion               
            }
            #region Chưa có địa chỉ giao hàng
            ConditionOrder.CreateOrder_C05(_context, _request, _apiCode, _conC04, _conC04Field);
            #endregion

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateOrderResponse> Process(CreateOrderRequest request)
        {
            try
            {
                request.UserId = new Guid("b542880b-f661-456a-9add-265b05c1b2bb");

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
