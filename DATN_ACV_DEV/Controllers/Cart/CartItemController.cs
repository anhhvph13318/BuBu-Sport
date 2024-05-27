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
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CartItem")]
    [ApiController]
    public class CartItemController : ControllerBase, IBaseController<CartItemRequest, CartItemResponse>
    {
        private readonly DBContext _context;
        private CartItemRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<CartItemResponse> _res;
        private CartItemResponse _response;
        private TbCart _Cart;
        private bool checkCart = false;
        private bool checkTimeCart = false;
        private string _apiCode = "CartItem";
        private string _conC01 = "C01";
        private string _conC01Field = "ProductID";
        public CartItemController(DBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _res = new BaseResponse<CartItemResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CartItemResponse();
            _Cart = new TbCart();
        }

        public void AccessDatabase()
        {
            List<CartDTO> LstCartItem = new List<CartDTO>();
            if (checkCart)
            {
                _context.Add(_Cart);
                _context.SaveChanges();
                _response.CartItem = LstCartItem;
            }
            else
            {
                if (_request.AdminId != null)
                {
                    _Cart = _context.TbCarts.Where(c => c.CreateBy == _request.AdminId).FirstOrDefault();
                }
                else
                {
                    _Cart = _context.TbCarts.Where(c => c.AccountId == _request.UserId).FirstOrDefault();
                }

                if (_Cart != null)
                {
                    var Model = _context.TbCartDetails.Include(a => a.Product).Where(c => c.CartId == _Cart.Id).ToList();
                    if (DateTime.Now > _Cart.EndDate)
                    {
                        Model.ForEach(c =>
                        {
                            c.Product.Quantity += c.Quantity.Value;
                        });
                        _context.RemoveRange(Model);
                        checkTimeCart = true;
                    }
                    if (!checkTimeCart)
                    {
                        if (Model != null)
                        {
                            var product = _context.TbProducts.Where(c => Model.Select(a => a.ProductId).Contains(c.Id)).Distinct().ToList();
                            var image = _context.TbImages.Where(i => product.Select(e => e.ImageId).Contains(i.Id)).Distinct().ToList();
                            Model.ForEach(c =>
                            {
                                c.tbProduct = product.Where(a => a.Id == c.ProductId).FirstOrDefault();
                                c.tbImage = image.Where(a => a.Id == c.tbProduct.ImageId).FirstOrDefault();
                            });
                        }
                        LstCartItem = _mapper.Map<List<CartDTO>>(Model);
                        _response.CartItem = LstCartItem;
                    }
                }
                _Cart.EndDate = DateTime.Now.AddDays(5);
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
            if (_request.AdminId == null)
            {
                var checkExistCart = _context.TbCarts.Where(c => c.AccountId == _request.UserId).FirstOrDefault();
                if (checkExistCart == null)
                {
                    _Cart = new TbCart()
                    {
                        Id = Guid.NewGuid(),
                        AccountId = _request.UserId,
                        CreateDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(5),
                    };
                    checkCart = true;
                }
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CartItemResponse> Process(CartItemRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
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
