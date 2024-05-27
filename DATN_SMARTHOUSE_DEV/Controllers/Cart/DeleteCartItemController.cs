using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/DeleteCartItem")]
    [ApiController]
    public class DeleteCartItemController : ControllerBase, IBaseController<DeleteCartItemRequest, DeleteCartItemResponse> 
    {
        private readonly DBContext _context;
        private DeleteCartItemRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<DeleteCartItemResponse> _res;
        private DeleteCartItemResponse _response;
        private TbCart _Cart;
        private bool checkCart = false;
        private string _apiCode = "DeleteCartItem";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "ProductID";
        private string _conC02Field = "Quantity";
        public DeleteCartItemController(DBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _res = new BaseResponse<DeleteCartItemResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DeleteCartItemResponse();
        }

        public void AccessDatabase()
        {
            var cartDetail = _context.TbCartDetails.Where(c => c.Id == _request.Id).FirstOrDefault();
            if (cartDetail != null)
            {
                _context.Remove(cartDetail);
                //var product = _context.TbProducts.Where(p => p.Id == cartDetail.ProductId).FirstOrDefault();
                //product.Quantity += cartDetail.Quantity.Value;
                _context.SaveChanges();
            }
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {

            if (_request.LoginType)
            {
                _request.Authorization(_context, _apiCode);
            }
            else
            {
                _request.AuthorizationCustomer(_context, _apiCode);
            }
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            #region sản phẩm trong giỏ hàng không tồn tại 
            ConditionCart.DeleteCartItem_C01(_context, _request.Id, _apiCode, _conC01, _conC01Field);
            #endregion
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<DeleteCartItemResponse> Process(DeleteCartItemRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
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
