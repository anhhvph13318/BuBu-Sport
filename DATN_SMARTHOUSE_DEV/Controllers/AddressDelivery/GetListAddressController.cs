using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.AddressDelivery;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListAddress")]
    [ApiController]
    public class GetListAddressController : ControllerBase, IBaseController<GetListAddressDeliveryRequest, GetListAddressDeliveryResponse>
    {
        private readonly DBContext _context;
        private GetListAddressDeliveryRequest _request;
        private BaseResponse<GetListAddressDeliveryResponse> _res;
        private GetListAddressDeliveryResponse _response;
        private readonly IMapper _mapper;
        private string _apiCode = "GetListAddress";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "ProductID";
        private string _conC02Field = "Quantity";
        public GetListAddressController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetListAddressDeliveryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListAddressDeliveryResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            var listAddress = _context.TbAddressDeliveries.Where(a => a.AccountId == _request.UserId && a.IsDelete == false);
            var query = _mapper.Map<List<AddressDeliveryDTO>>(listAddress.ToList());
            _response.listAddress = query;
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
        public BaseResponse<GetListAddressDeliveryResponse> Process(GetListAddressDeliveryRequest request)
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
