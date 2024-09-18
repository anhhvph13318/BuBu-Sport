using System.Web;
using GUI.Models.Order_DTO;
using GUI.Controllers.Shared;
using GUI.FileBase;
using GUI.Models.DTOs.Address;
using GUI.Models.DTOs.Cart_DTO;
using GUI.Models.DTOs.Product_DTO;
using GUI.Models.DTOs.Product_DTO.Views;
using GUI.Shared;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Security.Policy;
using GUI.Shared.VNPay;
using Microsoft.AspNetCore.Authorization;
using System;
using GUI.Models.DTOs.Order_DTO;
using GUI.Models.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Voucher_DTO;
using DATN_ACV_DEV.Entity;
using GUI.Models.DTOs.Customer_DTO.Views;
using Microsoft.Identity.Client;
using GUI.Models.DTOs.Customer_DTO;
using System.Net;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using AccountCustomerResponse = GUI.Models.Customer_DTO.AccountCustomerResponse;
using AccountCustomerRequest = GUI.Models.Customer_DTO.AccountCustomerRequest;
using EditCustomerResponse = GUI.Models.DTOs.Customer_DTO.EditCustomerResponse;
using EditCustomerRequest = GUI.Models.DTOs.Customer_DTO.EditCustomerRequest;

namespace GUI.Controllers
{
	[Authorize(Roles = "Guest")]
	[AllowAnonymous]
	public class StorefrontController : ControllerSharedBase
	{
		private HttpService httpService;
		private VNPayService _VNPayService;
		public StorefrontController(IOptions<CommonSettings> settings, VNPayService payService)
		{
			_settings = settings.Value;
			httpService = new();
			_VNPayService = payService;
		}

        [Route("/Home")]
		public IActionResult Index()
		{
			return View();
		}

		[Route("/Success")]
		public IActionResult Success(string vnp_TxnRef, string vnp_TransactionStatus, string vnp_SecureHash)
		{
			ViewBag.OrderId = vnp_TxnRef;
			if (vnp_TransactionStatus == "00" && !string.IsNullOrEmpty(vnp_SecureHash))
			{
				var request = new OrderStatusRequest
				{
					orderId = new Guid(vnp_TxnRef),
					paymentMethod = 2,
					paymentStatus = 1
				};
				var URL = _settings.APIAddress + "api/ConfirmPayment/Process";
				var param = JsonConvert.SerializeObject(request);
				httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			}

            return View();
		}

		[Route("/Store")]
		public async Task<IActionResult> Store(string s, int p, int t, decimal? min, decimal? max)
		{
			var model = new Models.DTOs.Product_DTO.Views.IndexObject();
			try
			{
				var obj = new GetListProductRequest();
				obj.Name = string.IsNullOrEmpty(s) ? "" : s;
				obj.PriceFrom = min;
				obj.PriceTo = max;
				obj.Limit = t <= 0 ? null : t;
				var offset = t * p;
				obj.OffSet = offset < 0 ? 0 : offset;
				var URL = _settings.APIAddress + "api/HomePage/Process";
				var param = JsonConvert.SerializeObject(obj);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();
				t = t == 0 ? 20 : t;
				var totalPages = ((result.Data.TotalCount) / t) - 1;
				totalPages = totalPages > 0 ? totalPages : 0;
				p = p >= totalPages ? totalPages : p;
				var topPageDisplay = 3;
				var startPage = p > 0 ? p - 1 : p;
				if (p > 0) {
					if (totalPages - p < 3)
					{
                        topPageDisplay = totalPages;
						startPage = totalPages - 3;
					}
					else
					{
						topPageDisplay = p + 2;
					}
				}

				ViewBag.SearchString = string.IsNullOrEmpty(s) ? "" : s;
				ViewBag.PriceFrom = min ?? result.Data.LowestPrice;
				ViewBag.PriceTo = max ?? result.Data.HighestPrice;
				ViewBag.Take = t <= 0 ? 20 : t;
				ViewBag.TakeOptions = new List<int>() { 10, 20, 50 };
				ViewBag.CurrentPage = p;
				ViewBag.TopPage = topPageDisplay;
				ViewBag.TotalPages = totalPages;
				ViewBag.StartPage = startPage;

				model.Data = result.Data;

				var accountId = HttpContext.Session.GetString("CurrentUserId");
				HttpContext.Session.Remove("CurrentUserId");
				if (!string.IsNullOrEmpty(accountId))
				{
					try
					{
						var req = new AccountCustomerRequest { Id = Guid.Parse(accountId) };
						var URLAcc = _settings.APIAddress + "api/AccountCustomer/Process";
						var paramAcc = JsonConvert.SerializeObject(req);
						var resAcc = await httpService.PostAsync(URLAcc, paramAcc, HttpMethod.Post, "application/json");
						var resultAcc = JsonConvert.DeserializeObject<BaseResponse<AccountCustomerResponse>>(resAcc) ?? new();
						if (resultAcc != null && resultAcc.Status == "200" && resultAcc.Data != null) {
							if (!resultAcc.Data.IsCustomer)
							{
								return RedirectToAction("Index", "Home");
							}
							var CustomerData = resultAcc.Data.Customer;
							if (CustomerData != null)
							{
								ViewBag.AccountId = accountId;
								ViewBag.CustomerName = CustomerData.Name ?? CustomerData.Phone;
								ViewBag.CustomerId = CustomerData.Id;
								ViewBag.CustomerPhone = CustomerData.Phone;
							}
						}
						else
						{
							return View(model);
						}
					}
					catch (Exception)
					{

					}
				}
			}
			catch (Exception)
			{
			}
			return View(model);
		}

		[Route("/OrderChecking")]
		public async Task<IActionResult> OrderChecking(string s)
		{
			List<OrderDetail> model = null;
			ViewBag.OrderSearch = s;

            if (!string.IsNullOrEmpty(s))
			{
				using var httpClient = new HttpClient();
				httpClient.BaseAddress = new Uri(_settings.APIAddress);
				var rawResponse = await httpClient.GetAsync($"/api/storefront/orders/search/{s}");
				try
				{
					var response =
								JsonConvert.DeserializeObject<BaseResponse<List<OrderDetail>>>(
									await rawResponse.Content.ReadAsStringAsync());
					model = response.Data;

                }
				catch (Exception)
				{

				}
			}
			return View(model);
		}

		[HttpPost("/ConfirmCart")]
		public async Task<JsonResult> ConfirmCart(List<Guid> ids)
		{
			var userId = Guid.Empty;
			try
			{
				userId = new Guid(Request.Cookies["user-id"]);
			}
			catch (Exception)
			{
			}
			try
			{
				var req = new CartItemRequest();
				//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
				req.UserId = userId;
				req.id = ids;
				var URL = _settings.APIAddress + "api/CartItem/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<CartItemResponse>>(res) ?? new();
				if (result.Status == "200")
				{
					var data = result.Data.CartItem.Where(c => ids.Contains(c.CartDetailID));
					TempData["ConfirmedCartItems"] = JsonConvert.SerializeObject(data);
					TempData.Keep("ConfirmedCartItems");
				}
				return Json(new
				{
					success = true
				});
			}
			catch (Exception)
			{
				return Json(new
				{
					success = false
				});
			}
		}

		[HttpPost("/AddCart")]
		public async Task<IActionResult> AddToCart(Guid prId, Guid userId)
		{
			if (userId == Guid.Empty)
			{
				userId = Guid.NewGuid();
			}
			var req = new AddToCartRequest();
			//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			req.UserId = userId;
			req.Quantity = 1;
			req.ProductId = prId;
			var URL = _settings.APIAddress + "api/AddToCart/Process";
			var param = JsonConvert.SerializeObject(req);
			var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			var result = JsonConvert.DeserializeObject<BaseResponse<AddToCartResponse>>(res) ?? new();
			if (result.Status == "200")
			{
				return Ok(new { userId });
			}
			return BadRequest();
		}

        [HttpPost("/BuyNow")]
        public async Task<IActionResult> BuyNow(Guid prId, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.NewGuid();
            }
            var req = new AddToCartRequest();
            //req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
            req.UserId = Guid.Empty;
            req.Quantity = 1;
            req.ProductId = prId;
			req.incre = false;
            var URL = _settings.APIAddress + "api/AddToCart/Process";
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<AddToCartResponse>>(res) ?? new();
            if (result.Status == "200")
            {
                await ConfirmCart(new List<Guid> { result.Data.ItemId });
                return Ok(new { userId });
            }
            return BadRequest();
        }

        [Route("/Checkout")]
		public async Task<IActionResult> Checkout()
		{
			if (TempData["ConfirmedCartItems"] != null)
			{
				HttpContext.Session.SetString("SelectedVoucher","");
				TempData.Keep("ConfirmedCartItems");
				List<CartDTO> model = JsonConvert.DeserializeObject<List<CartDTO>>(TempData["ConfirmedCartItems"].ToString());
				var sum = model.Sum(c => c.Price * c.Quantity);
				if (sum > 0)
				{
					ViewBag.Sum = sum;
					return View(model);
				}
			}
			return RedirectToAction(nameof(Cart));
		}

		[Route("/Cart")]
		public async Task<IActionResult> Cart()
		{
			var a = GetClientIP(HttpContext);

			var userId = Guid.Empty;
			try
			{
				userId = new Guid(Request.Cookies["user-id"]);
			}
			catch (Exception)
			{
			}
			var model = new List<CartDTO>();
			if (userId != Guid.Empty)
			{
				var req = new CartItemRequest();
				req.UserId = userId;
				//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
				var URL = _settings.APIAddress + "api/CartItem/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<CartItemResponse>>(res) ?? new();
				if (result.Status == "200")
				{
					model = result.Data.CartItem;
				}
			}
			return View(model);
		}

		[HttpPost("/DeleteItem")]
		public async Task<IActionResult> DeleteItem(Guid id)
		{
			//List<CartDTO> model = JsonConvert.DeserializeObject<List<CartDTO>>(TempData["ConfirmedCartItems"].ToString());
			//if (model != null)
			//{
			//	model.RemoveAll(c => c.CartDetailID == id);
			//}
			//TempData["ConfirmedCartItems"] = JsonConvert.SerializeObject(model);
			var req = new DeleteCartItemRequest();
			//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			req.Id = id;
			var URL = _settings.APIAddress + "api/DeleteCartItem/Process";
			var param = JsonConvert.SerializeObject(req);
			var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			var result = JsonConvert.DeserializeObject<BaseResponse<AddToCartResponse>>(res) ?? new();
			if (result.Status == "200")
			{
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("/Buy")]
		public async Task<IActionResult> CreateOrder(CreateOrderObject obj)
		{
			var userId = Guid.Empty;
			try
			{
				userId = new Guid(Request.Cookies["user-id"]);
			}
			catch (Exception)
			{
			}
			List<Guid> cartDetails = obj.ids;
			Guid addrId;
			var addressReq = new CreateAddessDeliveryRequest
			{
				receiverName = obj.name ?? "",
				receiverPhone = obj.phone ?? "",
				status = true,
				provinceId = 1,
				districId = 1,
				wardName = obj.address ?? "",
				districName = obj.district ?? "",
				provinceName = obj.city ?? ""
			};
			//addressReq.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			addressReq.UserId = userId;
			var URL = _settings.APIAddress + "api/CreateAddress/Process";
			var paramAdd = JsonConvert.SerializeObject(addressReq);
			var resAdd = await httpService.PostAsync(URL, paramAdd, HttpMethod.Post, "application/json");
			var resultAdd = JsonConvert.DeserializeObject<BaseResponse<CreateAddessDeliveryResponse>>(resAdd) ?? new();
			if (resultAdd.Status == "200")
			{
				addrId = resultAdd.Data.id;
			}
			else
			{
				addrId = new Guid();
			}
            
            if (cartDetails != null && cartDetails.Any())
			{
				var sum = 0m;
                if (TempData["ConfirmedCartItems"] != null)
                {
                    List<CartDTO> model = JsonConvert.DeserializeObject<List<CartDTO>>(TempData["ConfirmedCartItems"].ToString());
                    sum = model.Sum(c => c.Price * c.Quantity);
                } else if (userId != Guid.Empty)
				{
					var reqItems = new CartItemRequest();
					reqItems.UserId = userId;
					//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
					URL = _settings.APIAddress + "api/CartItem/Process";
					var paramItems = JsonConvert.SerializeObject(reqItems);
					var resItems = await httpService.PostAsync(URL, paramItems, HttpMethod.Post, "application/json");
					var resultItems = JsonConvert.DeserializeObject<BaseResponse<CartItemResponse>>(resItems) ?? new();
					try
					{
						if (resultItems.Status == "200")
						{
							sum = resultItems.Data.CartItem.Sum(c => c.Price * c.Quantity);
						}
					}
					catch (Exception)
					{
					}
				}

				sum += obj.getatstore ? 0 : 30000;

				var voucherString = HttpContext.Session.GetString("SelectedVoucher");
				var discountAmount = 0m;
				if (!string.IsNullOrEmpty(voucherString))
				{
					var voucher = JsonConvert.DeserializeObject<VoucherDTO>(voucherString);
					if (voucher.Unit == VoucherUnit.Percent)
					{
						discountAmount = sum / 100 * voucher.Discount;
						discountAmount = discountAmount <= voucher.MaxDiscount ? discountAmount : voucher.MaxDiscount;
					}
					else
					{
						discountAmount = voucher.Discount;
					}
				}

				var req = new OrderRequest
				{
					cartDetailId = cartDetails,
					description = obj.note ?? "",
					addressDeliveryId = addrId,
					paymentMethodId = obj.isVNP ? 2 : 1,
					totalAmount = sum,
					UserId = userId,
					phoneNummber = obj.phone,
					addressDelivery = obj.address,
					name = obj.name,
					getAtStore = obj.getatstore,
					amountShip = obj.getatstore ? 30000 : 0,
					totalAmountDiscount = discountAmount
				};
				URL = _settings.APIAddress + "api/ConfirmOrder/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<OrderResponse>>(res) ?? new();
				if (result.Status == "200")
				{
					TempData.Remove("ConfirmedCartItems");
					if (obj.isVNP)
					{
						var ipAddr = GetClientIP(HttpContext);
						var url = _VNPayService.SendRequest(ipAddr, result.Data.id.ToString(), sum);
						return Ok(new
						{
							success = true,
							redirect = true,
							url = url
						});
					}
					else
					{
						return Ok(new
						{
							success = true,
							redirect = false,
							orderId = result.Data.id,
						});
					}
				}
			}
			return BadRequest(new
			{
				success = false
			});
		}

		[HttpPost("/ChangeQuantity")]
		public async Task<JsonResult> ChangeQuantity(UpdateCartItem obj)
		{
			try
			{
				var req = new EditCartRequest()
				{
					CartDetaiID = new Guid(obj.cartDetaiID),
					Quantity = obj.quantity,
					IsIncrement = obj.isIncrement
				};
				var URL = _settings.APIAddress + "api/EditCartItem/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<EditCartResponse>>(res) ?? new();
				if (result.Status == "200")
				{
					return Json(new
					{
						success = true,
						data = result.Data
					});
				}
			}
			catch (Exception)
			{
			}
			return Json(new
			{
				success = false
			});
		}

		[HttpPost("/LogOutStorefront")]
		public IActionResult LogOut()
		{
			TempData.Clear();
			HttpContext.Session.Remove("CurrentUserId");
			return Ok();
		}

		[HttpPost("ApplyVoucher")]
		public async Task<JsonResult> ApplyVoucher(Guid vId)
		{
			try
			{
				var req = new DetailVoucherRequest();
				//req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
				req.ID = vId;
				var URL = _settings.APIAddress + "api/DetailVoucher/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<DetailVoucherResponse>>(res) ?? new();
				if (result.Status == "200")
				{
					var detail = result.Data.voucherDetail;
					HttpContext.Session.SetString("SelectedVoucher", JsonConvert.SerializeObject(detail));
					return Json(new
					{
						success = true,
						code = detail.Code,
						discount = detail.Discount,
						maxDiscount = detail.Unit == VoucherUnit.Percent ? detail.MaxDiscount : -1,
					});
					//return Ok(new { userId });
				}
				return Json(new
				{
					success = false,
				});
			}
			catch (Exception)
			{
				return Json(new
				{
					success = false,
				});
			}
		}

		[Route("Customer")]
		public async Task<IActionResult> CustomerDetail() {
			var model = new CustomerInfoModel();
			try
			{
				var accountId = HttpContext.Request.Cookies["aId"];
				var phoneNumber = HttpContext.Request.Cookies["cPhone"];
				if (!string.IsNullOrEmpty(accountId))
				{
					var req = new AccountCustomerRequest { Id = Guid.Parse(accountId) };
					var URLAcc = _settings.APIAddress + "api/AccountCustomer/Process";
					var paramAcc = JsonConvert.SerializeObject(req);
					var resAcc = await httpService.PostAsync(URLAcc, paramAcc, HttpMethod.Post, "application/json");
					var resultAcc = JsonConvert.DeserializeObject<BaseResponse<AccountCustomerResponse>>(resAcc) ?? new();
					if (resultAcc != null && resultAcc.Status == "200" && resultAcc.Data != null)
					{
						if (!resultAcc.Data.IsCustomer)
						{
							return RedirectToAction("Index", "Home");
						}
						var CustomerData = resultAcc.Data.Customer;
						if (CustomerData != null)
						{
							TempData["OldPassword"] = CustomerData.Password;
							var info = new PersonalInfo
							{
								Phone = CustomerData.Phone,
								Name = CustomerData.Name,
								Sex = CustomerData.Sex,
								Dob = CustomerData.YearOfBirth,
								Address = CustomerData.Adress,
								Email = CustomerData.Email,
							};
							model.Info = info;
						}
					}

					using var httpClient = new HttpClient();
					httpClient.BaseAddress = new Uri(_settings.APIAddress);
					var rawResponse = await httpClient.GetAsync($"/api/storefront/orders/search/{phoneNumber}");
					try
					{
						var response =
									JsonConvert.DeserializeObject<BaseResponse<List<OrderDetail>>>(
										await rawResponse.Content.ReadAsStringAsync());
						model.Orders = response.Data;

					}
					catch (Exception)
					{

					}
				}
				else
				{
					return RedirectToAction("Store");
				}
			}
			catch (Exception)
			{

			}
			return View("Customer", model);
		}

		[HttpPost("UpdateCustomerInfo")]
		public async Task<JsonResult> UpdateCustomerInfo(string name, int sex, string address, string email)
		{
			try
			{
				var userId = HttpContext.Request.Cookies["user-id"];
				if (!string.IsNullOrEmpty(userId))
				{
					var req = new EditCustomerRequest { Id = Guid.Parse(userId), Name = name, Sex = sex, Adress = address, Email = email };
					var URLAcc = _settings.APIAddress + "api/EditCustomer/Process";
					var paramAcc = JsonConvert.SerializeObject(req);
					var resAcc = await httpService.PostAsync(URLAcc, paramAcc, HttpMethod.Post, "application/json");
					var resultAcc = JsonConvert.DeserializeObject<BaseResponse<EditCustomerResponse>>(resAcc) ?? new();
					if (resultAcc != null && resultAcc.Status == "200")
					{
						return Json(new
						{
							success = true,
						});
					}
				}
			}
			catch (Exception)
			{}
			return Json(new
			{
				success = false
			});
		}

		[HttpPost("UpdatePassword")]
		public async Task<JsonResult> UpdatePassword(string oldPassword, string password)
		{
			try
			{
				if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(password))
				{
					throw new ArgumentNullException(nameof(oldPassword));
				}
				var oldPw = TempData["OldPassword"]?.ToString();
				if (oldPassword != oldPw)
				{
					return Json(new
					{
						success = false,
						wrong = true,
					});
				}
				else
				{
					var userId = HttpContext.Request.Cookies["user-id"];
					if (!string.IsNullOrEmpty(userId))
					{
						var req = new ChangePasswordRequest { Id = Guid.Parse(userId), Password = password };
						var URLAcc = _settings.APIAddress + "api/ChangePassword/Process";
						var paramAcc = JsonConvert.SerializeObject(req);
						var resAcc = await httpService.PostAsync(URLAcc, paramAcc, HttpMethod.Post, "application/json");
						var resultAcc = JsonConvert.DeserializeObject<BaseResponse<ChangePasswordResponse>>(resAcc) ?? new();
						if (resultAcc != null && resultAcc.Status == "200")
						{
							TempData["OldPassword"] = password;
							return Json(new
							{
								success = true,
							});
						}
					}
					throw new Exception();
				}
			}
			catch (Exception)
			{
				return Json(new
				{
					success = false,
					wrong = false,
				});
			}
		}
		public class CreateOrderObject
		{
            public string? name { get; set; }
            public string? phone { get; set; }
			public string? address { get; set; }
            public string? district { get; set; }
            public string? city { get; set; }
			public string? note { get; set; }
			public bool getatstore { get; set; }
			public bool isVNP { get; set; }
			public List<Guid> ids { get; set; } = new();
        }

        public class UpdateCartItem
        {
			public string cartDetaiID { get; set; }
			public int? quantity { get; set; }
			public bool isIncrement { get; set; }
		}

		private static string GetClientIP(HttpContext httpContext)
		{
			return httpContext.Connection.RemoteIpAddress?.ToString();
		}
	}
}
