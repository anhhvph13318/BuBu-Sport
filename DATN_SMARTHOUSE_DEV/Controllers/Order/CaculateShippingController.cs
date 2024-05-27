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
using Microsoft.IdentityModel.Tokens;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CaculateShipping")]
    [ApiController]
    public class CaculateShippingController : ControllerBase, IBaseController<CaculateShippingRequest, CaculateShippingResponse>
    {
        private readonly DBContext _context;
        private CaculateShippingRequest _request;
        private BaseResponse<CaculateShippingResponse> _res;
        private CaculateShippingResponse _response;
        private string _apiCode = "CaculateShipping";
        public CaculateShippingController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CaculateShippingResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CaculateShippingResponse();
        }

        public void AccessDatabase()
        {

            GHNFeeRequest requesFee = new GHNFeeRequest()
            {
                service_type_id = Utility.Utility.SERVICE_TYPE_DEFAULT,
                insurance_value = _request.totalAmountOrder,
                to_ward_code = _request.WardCode,
                to_district_id = _request.districId,
                from_district_id = Utility.Utility.FORM_DISTRICT_ID_DEFAULT,
                // fix cứng
                weight = 10,
                lenght = 20,
                width = 20,
                height = 20,
            };
            _response.amountShip = Common.GetFee(Utility.Utility.tokenGHN, requesFee);
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
        public BaseResponse<CaculateShippingResponse> Process(CaculateShippingRequest request)
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
