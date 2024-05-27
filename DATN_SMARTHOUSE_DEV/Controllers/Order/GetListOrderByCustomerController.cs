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
using Microsoft.IdentityModel.Tokens;

namespace DATN_ACV_DEV.Controllers.Order
{
    [Route("api/GetListOrderByCustomer")]
    [ApiController]
    public class GetListOrderByCustomerController : ControllerBase, IBaseController<GetListOrderByCustomerRequest, List<GetListOrderByCustomerResponse>>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListOrderByCustomerRequest _request;
        private BaseResponse<List<GetListOrderByCustomerResponse>> _res;
        private List<GetListOrderByCustomerResponse> _response;
        private string _apiCode = "GetListOrderByCustomer";
        public GetListOrderByCustomerController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<List<GetListOrderByCustomerResponse>>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new List<GetListOrderByCustomerResponse>();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            var model = _context.TbOrders.Include(a => a.TbOrderDetails)
                .Where(c => c.CreateBy == _request.UserId /*&& c.Status != Utility.Utility.ORDER_STATUS_DONE*/)
                .Select(s => new GetListOrderByCustomerResponse
                {
                    id = s.Id,
                    orderCode = s.OrderCode,
                    amountShip = s.AmountShip,
                    totalProduct = s.TotalAmount,
                    totalAmount = (s.TotalAmount + s.AmountShip),
                    status = Common.ConvertStatusOrder(s.Status.Value),
                }).ToList();
            var orderDetails = _context.TbOrderDetails.Include(sp => sp.Product).Where(c => model.Select(a => a.id).Contains(c.OrderId)).Select(s => new OrderDetailProduct
            {
                orderId = s.OrderId,
                productId = s.ProductId,
                productName = s.Product.Name,
                price = s.Product.Price,
                quantity = s.Quantity,

            }).ToList();
            model.ForEach(action =>
            {
                action.products = orderDetails.Where(c => c.orderId == action.id).ToList();
            });
            _response = model;
            _res.Data = _response;
        }
        public void CheckAuthorization()
        {
            _request.AuthorizationCustomer(_context, _apiCode);

        }
        public void GenerateObjects()
        {
        }

        public void PreValidation()
        {
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<List<GetListOrderByCustomerResponse>> Process(GetListOrderByCustomerRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
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
