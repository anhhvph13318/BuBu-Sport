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
    [Route("api/ConfirmOrder")]
    [ApiController]
    public class ConfirmOrderController : ControllerBase, IBaseController<OrderRequest, OrderResponse>
    {
        private readonly DBContext _context;
        private OrderRequest _request;
        private BaseResponse<OrderResponse> _res;
        private OrderResponse _response;
        private string _apiCode = "ConfirmOrder";
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
        public ConfirmOrderController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<OrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new OrderResponse();
            _listProduct = new List<OrderProduct>();
            _lstOrderDetail = new List<TbOrderDetail>();
            _listCartDetail = new List<TbCartDetail>();
            _lstVoucherCode = new List<string>();
        }

        public void AccessDatabase()
        {
            if (AddCustomer) _context.Add(customer); else _context.Update(customer);
            _context.Add(_order);
            _context.AddRange(_lstOrderDetail);
            _context.RemoveRange(_listCartDetail);


			_response.id = _order.Id;
            _response.orderCode = _order.OrderCode;
            _response.voucherCode = string.Join(", ", _lstVoucherCode);
            _response.accountCode = account?.AccountCode;
            _response.phoneNumber = account?.PhoneNumber ?? _order.PhoneNumberCustomer;
            _response.amountShip = _order.AmountShip ?? 0m;
            _response.totalAmount = _order.TotalAmount;
            _response.totalAmountDiscount = _order.TotalAmountDiscount ?? 0m;
            _response.createdDate = _order.CreateDate;
            _response.PaymentMethodName = _namePaymentMethod;
            _response.nameCustomer = customer?.Name;
            _response.addressDelivery = _request.addressDelivery;
            _res.Data = _response;
            if (_request.voucherID != null)
            {
                var listVoucher = _context.TbVouchers.Where(v => _request.voucherID.Contains(v.Id));
                foreach (var voucher in listVoucher)
                {
                    voucher.Quantity -= 1;
                }
            }
            _context.SaveChanges();
        }

        public void CheckAuthorization()
        {

            _request.AuthorizationCustomer(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            customer = _context.TbCustomers.FirstOrDefault(c => c.Id == _request.UserId);
            if (customer == null)
            {
                //var address = _context.TbAddressDeliveries.FirstOrDefault(c => c.Id == _request.addressDeliveryId);
                customer = new TbCustomer { 
                    Id = _request.UserId,
                    Adress = _request.addressDelivery,
                    GroupCustomerId = _request.UserId,
                    Name = _request.name,
                    Phone = _request.phoneNummber,
				};
                AddCustomer = true;

			}
            //_namePaymentMethod = _context.TbPaymentMethods.Where(c => c.Id == _request.paymentMethodId).Select(p => p.Name).FirstOrDefault();
            _order = new TbOrder()
            {
                Id = Guid.NewGuid(),
                OrderCode = $"ON{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                TotalAmount = _request.totalAmount ?? 0m,
                TotalAmountDiscount = _request.totalAmountDiscount ?? 0m,
				Description = _request.description,
                AccountId = _request.UserId,
                PaymentMethod = _request.paymentMethodId ?? 1,
                PaymentStatus = (short)(_request.paymentMethodId == 2 ? 0 : 1),
                //VoucherCode = _request.voucherCode != null ? string.Join(",", _request.voucherCode) : null,
                AmountShip = _request.amountShip ?? 0,
                CustomerId = customer == null ? null : customer.Id,
                PhoneNumberCustomer = account != null ? account.PhoneNumber : _request.phoneNummber,
                AddressDeliveryId = _request.addressDeliveryId,
                IsCustomerTakeYourself = _request.getAtStore == true,

				OrderCounter = false,
                //Defautl
                CreateBy = _request.AdminId ?? _request.UserId,
                CreateDate = DateTime.Now,
                Status = Utility.Utility.ORDER_STATUS_PREPARE_GOODS
            };
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
                        //_order.TotalAmount += (model.Quantity * model.Price);
						_listProduct.Add(product);
					}
				}
			}
			foreach (var i in _listProduct)
            {
                _orderDetail = new TbOrderDetail()
                {
                    Id = Guid.NewGuid(),
                    OrderId = _order.Id,
                    ProductId = i.productId,
                    Quantity = i.quantity
                };
                _lstOrderDetail.Add(_orderDetail);
                _response.products.Add(i);
            }

            //_order.TotalAmount = 
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
                        ACV_Exception.Messages.Add(Message.CreateErrorMessage(_apiCode, _conC02, "Sản phẩm có mã :" + model.Code ?? "" + Utility.Utility.PRODUCT_NOTFOUND, _conC02Field));
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
            }

            #region Chưa có địa chỉ giao hàng
            // ConditionOrder.CreateOrder_C05(_context, _request.UserId, _apiCode, _conC04, _conC04Field);
            #endregion

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<OrderResponse> Process([FromBody]OrderRequest request)
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
