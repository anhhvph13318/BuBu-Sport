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
    [Route("api/GetListOrderAdmin")]
    [ApiController]
    public class GetListOrderAdminController : ControllerBase, IBaseController<GetListOrderAdminRequest, GetListOrderAdminResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListOrderAdminRequest _request;
        private BaseResponse<GetListOrderAdminResponse> _res;
        private GetListOrderAdminResponse _response;
        private string _apiCode = "GetListOrderAdmin";
        public GetListOrderAdminController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetListOrderAdminResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListOrderAdminResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            List<GetListOrderAdminDTO> lstOrder = new List<GetListOrderAdminDTO>();
            var model = _context.TbOrders.Where(c => c.Id != null
                                                  && (!string.IsNullOrEmpty(_request.code) ? c.OrderCode.Contains(_request.code) : true)
                                                   && (!string.IsNullOrEmpty(_request.phoneNumberCustomer) ? c.PhoneNumberCustomer.Contains(_request.phoneNumberCustomer) : true)
                                                  && (_request.orderType.HasValue ? c.OrderCounter == _request.orderType : true)
                                                  && (_request.status.HasValue ? c.Status == _request.status : true)
                                                  && (_request.fromDate.HasValue ? c.CreateDate >= _request.fromDate : true)
                                                  && (_request.fromDate.HasValue ? c.CreateDate <= _request.toDate : true))
                                                  .OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var customer = _context.TbCustomers.Where(c => model.Select(s => s.CustomerId).Contains(c.Id)).Distinct();
            var paymentMenthod = _context.TbPaymentMethods.Where(c => model.Select(s => s.PaymentMethodId).Contains(c.Id)).Distinct();
            var orderDetails = _context.TbOrderDetails.Include(product => product.Product).Distinct();
            model.ToList().ForEach(c =>
            {
                c.customer = customer.Where(a => a.Id == c.CustomerId).FirstOrDefault();
                c.paymentMethod = paymentMenthod.Where(a => a.Id == c.PaymentMethodId).FirstOrDefault();
                c.orderDetail = orderDetails.Where(d => d.OrderId == c.Id).ToList();
            });

            var query = _mapper.Map<List<GetListOrderAdminDTO>>(model);
            if (_request.Limit == null)
            {
                lstOrder = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                lstOrder = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.Orders = lstOrder;
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
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GetListOrderAdminResponse> Process(GetListOrderAdminRequest request)
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
