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
    [Route("api/CreateAddress")]
    [ApiController]
    public class CreateAddressController : ControllerBase, IBaseController<CreateAddessDeliveryRequest, CreateAddessDeliveryResponse>
    {
        private readonly DBContext _context;
        private CreateAddessDeliveryRequest _request;
        private BaseResponse<CreateAddessDeliveryResponse> _res;
        private CreateAddessDeliveryResponse _response;
        private TbAddressDelivery _addressDelivery;
        private bool AddUser = false;
        private string _apiCode = "CreateAddress";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "ProductID";
        private string _conC02Field = "Quantity";
        public CreateAddressController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateAddessDeliveryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateAddessDeliveryResponse();
        }

        public void AccessDatabase()
        {
            _context.TbAddressDeliveries.Add(_addressDelivery);
            _context.SaveChanges();
            _response.id = _addressDelivery.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.AuthorizationCustomer(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            //var checkUser = _context.TbAccounts.FirstOrDefault(c => c.Id == _request.UserId);
            _addressDelivery = new TbAddressDelivery
            {
                Id = Guid.NewGuid(),
                ProvinceName = _request.provinceName,
                //DistrictName = _request.districName,
                //WardName = _request.wardName,
                //ProviceId = _request.provinceId,
                //DistrictId = _request.districId,
                Status = _request.status,
                //AccountId = _request.UserId,
                ReceiverName = _request.receiverName,
                ReceiverPhone = _request.receiverPhone,
                IsDelete = false,
            };
        }

        public void PreValidation()
        {
        }

        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateAddessDeliveryResponse> Process(CreateAddessDeliveryRequest request)
        {
            try
            {
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
