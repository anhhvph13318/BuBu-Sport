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
    [Route("api/CreateOrderCounter")]
    [ApiController]
    public class CreateOrderCounterController : ControllerBase, IBaseController<CreateOrderCounterRequest, CreateOrderCounterResponse>
    {
        private readonly DBContext _context;
        private CreateOrderCounterRequest _request;
        private BaseResponse<CreateOrderCounterResponse> _res;
        private CreateOrderCounterResponse _response;
        private List<TbCartDetail> _listCartDetail;
        private List<OrderProduct> _listProduct;
        private string? _VoucherCode;
        private Guid? _VoucherId;
        private string _apiCode = "CreateOrderCounter";

        private decimal? _totalDiscount = 0;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC03 = "C03";
        private string _conC04 = "C04";
        private string _conC01Field = "cartDetailID";
        private string _conC02Field = "ProductID";
        private string _conC03Field = "VoucherID";
        private string _conC04Field = "VoucherID";
        private decimal? _totalAmount = 0;
        public CreateOrderCounterController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateOrderCounterResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateOrderCounterResponse();
            _listCartDetail = new List<TbCartDetail>();
            _listProduct = new List<OrderProduct>();
        }

        public void AccessDatabase()
        {
            //var paymentMethod = _context.TbPaymentMethods.Where(p => p.Id == _request.paymentMenthodID).FirstOrDefault();
            //_response.products = _listProduct;
            //_response.totalAmount = _totalAmount;
            //_response.totalAmountDiscount = _totalDiscount;
            //_response.voucherCode = _VoucherCode;
            //_response.cartDetailId = _request.cartDetailID;
            //_response.voucherId = _VoucherId;
            //if (paymentMethod != null)
            //{
            //    _response.paymentMethod = paymentMethod.Name;
            //}
            //_res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            //foreach (var product in _listProduct)
            //{
            //    if (_request.voucherCode != null)
            //    {
            //        var voucher = _context.TbVouchers.Where(voucher => voucher.Code == _request.voucherCode).FirstOrDefault();
            //        if (voucher.ProductId == product.productId)
            //        {
            //            var totalAmountPromotionApplyProduct = product.price * product.quantity;
            //            var amountDiscount = Common.CalculateDiscount(totalAmountPromotionApplyProduct, voucher);
            //            _totalAmount += (totalAmountPromotionApplyProduct - amountDiscount.DiscountVoucher);
            //            _totalDiscount += amountDiscount.DiscountVoucher;
            //            _VoucherCode = voucher.Code;
            //            _VoucherId = voucher.Id;
            //        }
            //        else if (voucher.CategoryId == product.categoryId)
            //        {
            //            var totalAmountPromotionApplyCategory = product.price * product.quantity;
            //            var amountDiscount = Common.CalculateDiscount(totalAmountPromotionApplyCategory, voucher);
            //            _totalAmount += (totalAmountPromotionApplyCategory - amountDiscount.DiscountVoucher);
            //            _totalDiscount = amountDiscount.DiscountVoucher;
            //            _VoucherCode = voucher.Code;
            //            _VoucherId = voucher.Id;
            //        }
            //        else
            //        {
            //            _totalAmount += (product.price * product.quantity);
            //        }

            //    }
            //    else
            //    {
            //        _totalAmount += (product.price * product.quantity);
            //    }
            //}
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
            if (_request.voucherCode != null)
            {

                #region voucher không tồn tại or voucher type không phải là discount
                ConditionOrder.CreateOrderCouter_C03(_context, _request.voucherCode, _apiCode, _conC04, _conC04Field);
                #endregion

                #region Không có sản phẩm nào trong hóa đơn được áp dụng voucher.
                ConditionOrder.CreateOrderCouter_C06(_context, _request, _apiCode, _conC04, _conC04Field);
                #endregion

            }
        }

        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateOrderCounterResponse> Process(CreateOrderCounterRequest request)
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
