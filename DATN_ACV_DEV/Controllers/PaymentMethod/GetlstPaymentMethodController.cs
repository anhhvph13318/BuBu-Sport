using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.PaymentMedthod_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.PaymentMethod
{
    [Route("api/GetlstPaymentMethod")]
    [ApiController]
    public class GetlstPaymentMethodController : ControllerBase, IBaseController<GetLstPaymentMethodRequest, GetLstPaymentMethodResponse> 
    {
        private readonly DBContext _context;
        private GetLstPaymentMethodRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<GetLstPaymentMethodResponse> _res;
        private GetLstPaymentMethodResponse _response;
        private string _apiCode = "GetlstPaymentMethod";
        private TbImage _Image;
        public GetlstPaymentMethodController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetLstPaymentMethodResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetLstPaymentMethodResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            var model = _context.TbPaymentMethods.Where(c => c.InActive == true);
            var query = _mapper.Map<List<PaymentMethodDTO>>(model.ToList());
            _response.getLstPaymentMethod = query;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GetLstPaymentMethodResponse> Process(GetLstPaymentMethodRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation();
                //GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex.Messages;
            }
            catch (System.Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;

        }
    }
}
