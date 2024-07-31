﻿
using GUI.Models.Order_DTO;
using GUI.Controllers.Shared;
using GUI.FileBase;
using GUI.Models.DTOs.Address;
using GUI.Models.DTOs.Cart_DTO;
using GUI.Models.DTOs.Product_DTO;
using GUI.Models.DTOs.Product_DTO.Views;
using GUI.Shared;
using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Security.Policy;

namespace GUI.Controllers
{
	public class StorefrontController : ControllerSharedBase
	{
		private HttpService httpService;
		public StorefrontController(IOptions<CommonSettings> settings)
		{
			_settings = settings.Value;
			httpService = new();
		}

        [Route("/")]
		public IActionResult Index()
		{
			return View();
		}

		[Route("/Store")]
		public async Task<IActionResult> Store(string s)
		{
			var obj = new GetListProductRequest();
			var model = new IndexObject();
			obj.Name = string.IsNullOrEmpty(s) ? "" : s;
			var URL = _settings.APIAddress + "api/HomePage/Process";
			var param = JsonConvert.SerializeObject(obj);
			var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();

			model.Data = result.Data;
			return View(model);
		}

		[HttpPost("/AddCart")]
		public async Task<IActionResult> AddToCart(Guid prId)
		{
			var req = new AddToCartRequest();
			req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			req.Quantity = 1;
			req.ProductId = prId;
			var URL = _settings.APIAddress + "api/AddToCart/Process";
			var param = JsonConvert.SerializeObject(req);
			var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			var result = JsonConvert.DeserializeObject<BaseResponse<AddToCartResponse>>(res) ?? new();
			if (result.Status == "200")
			{
				return Ok();
			}
			return BadRequest();
		}

		[Route("/Checkout")]
		public async Task<IActionResult> Checkout()
		{
			var model = new List<CartDTO>();
			var sum = 0m;
			var req = new CartItemRequest();
			req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			var URL = _settings.APIAddress + "api/CartItem/Process";
			var param = JsonConvert.SerializeObject(req);
			var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
			var result = JsonConvert.DeserializeObject<BaseResponse<CartItemResponse>>(res) ?? new();
			if (result.Status == "200")
			{
				model = result.Data.CartItem;
				sum = model.Sum(c => c.Price * c.Quantity);
			}
			ViewBag.Sum = sum;
			return View(model);
		}

		[HttpPost("/DeleteItem")]
		public async Task<IActionResult> DeleteItem(Guid id)
		{
			var req = new DeleteCartItemRequest();
			req.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
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
			Guid addrId;
			List<Guid> cartDetails = new();
			var addressReq = new CreateAddessDeliveryRequest
			{
				receiverName = obj.name ?? "",
				receiverPhone = obj.phone ?? "",
				status = true,
				provinceId = 1,
				districId = 1,
				wardName = obj.address ?? "",
				wardCode = obj.zipCode ?? "",
				districName = obj.district ?? "",
				provinceName = obj.city ?? ""
			};
			addressReq.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
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
			var reqItems = new CartItemRequest();
			reqItems.UserId = new Guid("6E55E6C4-69F8-43A9-B5B7-00216EC0B0AD");
			URL = _settings.APIAddress + "api/CartItem/Process";
			var paramItems = JsonConvert.SerializeObject(reqItems);
			var resItems = await httpService.PostAsync(URL, paramItems, HttpMethod.Post, "application/json");
			var resultItems = JsonConvert.DeserializeObject<BaseResponse<CartItemResponse>>(resItems) ?? new();
			var sum = 0m;
			try
			{
				sum = resultItems.Data.CartItem.Sum(c => c.Quantity * c.Price);
            }
			catch (Exception)
			{
			}
			if (resultItems.Status == "200")
			{
				cartDetails = resultItems.Data.CartItem.Select(c => c.CartDetailID).ToList();
			}
			if (cartDetails != null && cartDetails.Any())
			{
				var req = new OrderRequest
				{
					cartDetailId = cartDetails,
					description = obj.note ?? "",
					addressDeliveryId = addrId,
					paymentMethodId = new Guid(),
					totalAmount = resultItems.Data.CartItem.Sum(c => c.Quantity * c.Price)
				};
				URL = _settings.APIAddress + "api/ConfirmOrder/Process";
				var param = JsonConvert.SerializeObject(req);
				var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
				var result = JsonConvert.DeserializeObject<BaseResponse<OrderResponse>>(resItems) ?? new();
				if (result.Status == "200")
				{
					return Ok(new
					{
						success = true
					});
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

		public class CreateOrderObject
		{
            public string? name { get; set; }
            public string? phone { get; set; }
			public string? address { get; set; }
            public string? district { get; set; }
            public string? city { get; set; }
			public string? zipCode { get; set; }
			public string? note { get; set; }
        }

        public class UpdateCartItem
        {
			public string cartDetaiID { get; set; }
			public int? quantity { get; set; }
			public bool isIncrement { get; set; }
		}
    }
}
