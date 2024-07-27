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
using System.Security.Principal;

namespace DATN_ACV_DEV.Controllers.GHN_API
{
    [Route("api/CreateOrderGHN")]
    [ApiController]
    public class CreateOrderGHNController : ControllerBase, IBaseController<GHNCreateOrderRequest, GHNCreateOrderResponse>
    {
        private readonly DBContext _context;
        private GHNCreateOrderRequest _request;
        private TbCustomer _customer;
        private TbAccount _account;
        private TbOrder _order;
        private string _conC01 = "C01";
        private string _conC01Field = "orderId";
        private TbAddressDelivery _addressDelivery;
        private RequestCreateOrderGHN _requestCreateOrderGHN;
        //private List<TbProduct> _products;
        private List<item> _lstItem;
        private BaseResponse<GHNCreateOrderResponse> _res;
        private GHNCreateOrderResponse _response;
        private string _apiCode = "CreateOrderGHN";
        public CreateOrderGHNController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<GHNCreateOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _customer = new TbCustomer();
            _addressDelivery = new TbAddressDelivery();
            _order = new TbOrder();
            //_products = new List<TbProduct>();
            _lstItem = new List<item>();
            _response = new GHNCreateOrderResponse();
        }

        public void AccessDatabase()
        {

        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);

        }

        public void GenerateObjects()
        {
            _order = _context.TbOrders.Where(o => o.Id == _request.orderId).FirstOrDefault();
            if (_order != null)
            {
                _customer = _context.TbCustomers.Where(c => c.Id == _order.CustomerId).FirstOrDefault();
                _account = _context.TbAccounts.Where(c => c.CustomerId == _customer.Id).FirstOrDefault();
                _addressDelivery = _context.TbAddressDeliveries.Where(a => a.Id == _order.AddressDeliveryId).FirstOrDefault();
            }
            var orderDetails = _context.TbOrderDetails.Where(d => d.OrderId == _order.Id).ToList();

            foreach (var item in orderDetails)
            {
                var product = _context.TbProducts.Where(p => p.Id == item.ProductId).FirstOrDefault();
                _lstItem.Add(new item
                {
                    name = product.Name,
                    code = product.Code,
                    quantity = item.Quantity,
                    weight = product.Weight ?? 5
                });
            }
            _requestCreateOrderGHN = new RequestCreateOrderGHN
            {
                to_name = _customer.Name ?? "Anh/Chị",
                to_phone = _account.PhoneNumber,
                to_address = $"{_addressDelivery.WardName}, {_addressDelivery.DistrictName}, {_addressDelivery.ProvinceName}",
                to_district_id = _addressDelivery.DistrictId,
                cod_amount = (int)_order.TotalAmount,
                weight = _lstItem.Sum(c => c.weight),
                length = 100,
                width = 100,
                height = 100,
                insurance_value = (int)_order.TotalAmount,
                payment_type_id = 1,
                service_id = 0,
                service_type_id = 2,
                items = _lstItem,

            };
            var responGHN = Common.CreateOderGHN(_requestCreateOrderGHN);
            if (responGHN.code == StatusCodes.Status200OK)
            {
                _order.Status = Utility.Utility.ORDER_STATUS_SHIPPED;
                _order.OrderCodeGhn = responGHN.data.order_code;
            }
            //Giamr số lượng sản phẩm lúc gửi hàng
            foreach (var item in orderDetails)
            {
                var product = _context.TbProducts.Where(p => p.Id == item.ProductId).FirstOrDefault();
                product.Quantity -= item.Quantity;
            }
            //đến đây
            _context.SaveChanges();
            _response.codeOrderGHN = _order.OrderCodeGhn;
            _res.Data = _response;
        }

        public void PreValidation()
        {
            // Đơn hàng không tồn tại
            Condition.ConditionOrder.CreateOrderGHN_C01(_context, _request, _apiCode, _conC01, _conC01Field);
            // Trạng thái đơn hàng không hợp lệ
            Condition.ConditionOrder.CreateOrderGHN_C02(_context, _request, _apiCode, _conC01, _conC01Field);
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GHNCreateOrderResponse> Process(GHNCreateOrderRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
                GenerateObjects();
                //PostValidation();
                //AccessDatabase();
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
