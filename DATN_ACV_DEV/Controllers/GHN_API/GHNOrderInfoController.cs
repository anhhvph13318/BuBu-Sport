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
    [Route("api/GHNOrderInfo")]
    [ApiController]
    public class GHNOrderInfoController : ControllerBase, IBaseController<GHNInfoOrderRequest, GHNInfoOrderResponse>
    {
        private readonly DBContext _context;
        private GHNInfoOrderRequest _request;
        private TbCustomer _customer;
        private TbAccount _account;
        private TbOrder _order;
        private TbAddressDelivery _addressDelivery;
        private RequestCreateOrderGHN _requestCreateOrderGHN;
        //private List<TbProduct> _products;
        private List<item> _lstItem;
        private BaseResponse<GHNInfoOrderResponse> _res;
        private GHNInfoOrderResponse _response;
        private string _apiCode = "GHNOrderInfo";
        public GHNOrderInfoController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<GHNInfoOrderResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _customer = new TbCustomer();
            _addressDelivery = new TbAddressDelivery();
            _order = new TbOrder();
            //_products = new List<TbProduct>();
            _lstItem = new List<item>();
            _response = new GHNInfoOrderResponse();
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
            
            var responGHN = Common.InfoOrderGHN(_request);
            _context.SaveChanges();          
            _res.Data = _response;
        }

        public void PreValidation()
        {

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GHNInfoOrderResponse> Process(GHNInfoOrderRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
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
