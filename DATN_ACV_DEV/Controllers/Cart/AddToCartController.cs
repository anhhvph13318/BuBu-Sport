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

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/AddToCart")]
    [ApiController]
    public class AddToCartController : ControllerBase, IBaseController<AddToCartRequest, AddToCartResponse>
    {
        private readonly DBContext _context;
        private AddToCartRequest _request;
        private BaseResponse<AddToCartResponse> _res;
        private AddToCartResponse _response;
        private string _apiCode = "AddToCart";
        private TbCart _Cart;
        private TbCartDetail _CartDetail;
        private TbProduct _product;
        private bool checkCart = false;
        private bool checkProduct = false;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "ProductID";
        private string _conC02Field = "Quantity";
        public AddToCartController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<AddToCartResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new AddToCartResponse();
        }

        public void AccessDatabase()
        {
            if (checkCart)
            {
                _context.Add(_Cart);
                _context.Add(_CartDetail);
            }
            if (checkProduct)
            {
                _context.Add(_CartDetail);
            }
            //_product = _context.TbProducts.Where(p => p.Id == _request.ProductId).FirstOrDefault();
            //_product.Quantity -= _request.Quantity;
            _context.SaveChanges();
            _response.ItemId = _CartDetail.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            //if (_request.LoginType)
            //{
            //    _request.Authorization(_context, _apiCode);
            //}
            //else
            //{
            //    _request.AuthorizationCustomer(_context, _apiCode);
            //}
        }
        public void AddCartAdmin()
        {
            _Cart = new TbCart()
            {
                Id = Guid.NewGuid(),
                AccountId = _request.UserId,
                CreateDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                CartAdmin = true,
                CreateBy = _request.AdminId
            };
            _CartDetail = new TbCartDetail()
            {
                Id = Guid.NewGuid(),
                ProductId = _request.ProductId,
                Quantity = _request.Quantity,
                CartId = _Cart.Id,
            };
            checkCart = true;
        }
        public void AddCartCustomer()
        {
            _Cart = new TbCart()
            {
                Id = Guid.NewGuid(),
                AccountId = _request.UserId,
                CreateDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                CartAdmin = false
            };
            _CartDetail = new TbCartDetail()
            {
                Id = Guid.NewGuid(),
                ProductId = _request.ProductId,
                Quantity = _request.Quantity,
                CartId = _Cart.Id,
            };
            checkCart = true;
        }
        public void GenerateObjects()
        {
            var checkExistCart = _context.TbCarts.Where(c => c.AccountId == _request.UserId).FirstOrDefault();
            if (checkExistCart == null)
            {
                if (_request.AdminId != null)
                {
                    AddCartAdmin();
                }
                else
                {
                    AddCartCustomer();
                }
                checkExistCart = _context.TbCarts.Where(c => c.AccountId == _request.UserId).FirstOrDefault();
            }
            else
            {
                try
                {
                    var LstproductID = _context.TbCartDetails.Where(c => c.CartId == checkExistCart.Id).ToList();
                    var Product = LstproductID.Where(c => c.ProductId == _request.ProductId).FirstOrDefault();
                    if (Product != null && _request.incre)
                    {
                        Product.Quantity += _request.Quantity;
                    }
                    else
                    {
                        _CartDetail = new TbCartDetail()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = _request.ProductId,
                            Quantity = _request.Quantity,
                            CartId = checkExistCart.Id,
                        };
                        checkProduct = true;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void PreValidation()
        {
            #region Sản phẩm không tồn tại 
            ConditionCart.AddToCart_C01(_context, _request, _apiCode, _conC01, _conC01Field);
            #endregion

            #region Số lượng sản phẩm không đủ 
            ConditionCart.AddToCart_C02(_context, _request, _apiCode, _conC02, _conC02Field);
            #endregion
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<AddToCartResponse> Process([FromBody]AddToCartRequest request)
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
