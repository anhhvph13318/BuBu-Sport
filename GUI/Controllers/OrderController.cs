using DATN_ACV_DEV.Controllers;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using GUI.FileBase;
using GUI.Hubs;
using GUI.Models.DTOs.Order_DTO;
using GUI.Models.DTOs.Voucher_DTO;
using GUI.Shared.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Globalization;
using Rotativa.AspNetCore;
using System.Net.WebSockets;
using OrderItem = DATN_ACV_DEV.Model_DTO.Order_DTO.OrderItem;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Controllers.Order;
using Microsoft.EntityFrameworkCore;
using static DATN_ACV_DEV.Controllers.Order.AdminCreateOrderController;

namespace GUI.Controllers;

[Controller]
[Route("orders")]
//[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly IEmailService _emailService;
    private const string URI = "http://localhost:5059";
    //private const string URI = "https://localhost:44383";
    private const string OrderItemListPartialView = "_OrderItemListPartialView";
    private const string OrderCustomerInfoPartialView = "_OrderCustomerInfoPartialView";
    private const string OrderPaymentInfoPartialView = "_OrderPaymentInfoPartialView";
    private const string OrderShippingInfoPartialView = "_OrderShippingInfoPartialView";
    private const string OrderButtonActionPartialView = "_OrderButtonActionPartialView";
    private const string OrderListPartialView = "_OrderListPartialView";
    private const string AvailableVoucherPartialView = "_AvailableVoucherPartialView";
    private const string TempSaveOrderButtonPartialView = "_TempSaveOrderButtonPartialView";
    public OrderController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    [HttpGet]
    public async Task<IActionResult> Index(
    [FromQuery] string? code = "",
    [FromQuery] string? customerName = "",
    [FromQuery] int status = -1,
    [FromQuery] decimal? minAmount = null,
    [FromQuery] decimal? maxAmount = null,
    [FromQuery] string? orderCodePrefix = "", 
    DateTime? startDate = null, 
    DateTime? endDate = null)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URI);
            var query = $"/api/admin/orders?code={code}&customerName={customerName}&status={status}";
            var rawResponse = await httpClient.GetAsync(query);

            if (rawResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Có lỗi khi gọi API: {rawResponse.StatusCode}");
            }

            var response = JsonConvert.DeserializeObject<BaseResponse<IEnumerable<OrderListItem>>>(
                await rawResponse.Content.ReadAsStringAsync());
            
            var orders = response!.Data;

            // Lọc theo orderCodePrefix
            if (!string.IsNullOrEmpty(orderCodePrefix))
            {
                orders = orders.Where(o => o.code.StartsWith(orderCodePrefix));
            }

            if (minAmount.HasValue)
            {
                orders = orders.Where(o => o.FinalAmount >= minAmount.Value);
            }
            if (maxAmount.HasValue)
            {
                orders = orders.Where(o => o.FinalAmount <= maxAmount.Value);
            }
            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.CreateDate >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.CreateDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }


            return View(orders.OrderByDescending(c=>c.CreateDate));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
            TempData["Error"] = "Có lỗi xảy ra khi tải danh sách hóa đơn.";
            return View(new List<OrderListItem>());
        }
    }

    [HttpGet]
    [Route("vouchers")]
    public async Task<IActionResult> GetAvailableVoucher([FromQuery] string? phone)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = string.IsNullOrEmpty(phone)
            ? await httpClient.GetAsync($"/api/vouchers/available")
            : await httpClient.GetAsync($"/api/vouchers/available?phoneNumber={phone}");

        var response =
            JsonConvert.DeserializeObject<BaseResponse<GetListVoucherResponse>>(
                await rawResponse.Content.ReadAsStringAsync());

        return Json(new
        {
            Vouchers = await RenderViewAsync(AvailableVoucherPartialView, response!.Data.LstVoucher)
        });
    }

    [HttpGet]
    [Route("/detaill/{id}")]
    public async Task<IActionResult> Detail(string id, bool? autoUpdate = false)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                await rawResponse.Content.ReadAsStringAsync());
        if (response.Data.Voucher == null)
        {
            response.Data.Voucher = new VoucherDTO();
        }
        var order = response!.Data;
        order.ReCalculatePaymentInfo();
        ViewBag.AutoUpdate = autoUpdate; // Gửi cờ này sang view
        return View(response!.Data);
    }
    [HttpGet]
    [Route("wait-stock")]
    public IActionResult WaitForStock(Guid orderId)
    {
        DBContext context = new DBContext();
        context = context ?? new DBContext();
        TbOrder Order = new TbOrder();
        Order = context.TbOrders.Where(c => c.Id == orderId).FirstOrDefault();
        Order.IsWait = "1";
        context.SaveChanges();
        return View(model: Order.OrderCode); // Hoặc trả JSON nếu là API thuần
    }
    [HttpGet]
    [Route("confirm-cancel")]
    public IActionResult ConfirmCancel(Guid orderId)
    {
        DBContext context = new DBContext();
        context = context ?? new DBContext();
        var order = context.TbOrders.Where(c => c.Id == orderId).FirstOrDefault();
        order.Status = 3;
        order.ReasionCancel = "Lựa chọn của khách hàng";
        context.SaveChanges();
        return View(model: order.OrderCode); // Hoặc trả JSON nếu là API thuần
    }
    [HttpGet]
    [Route("confirm-partial")]
    public IActionResult ConfirmPartial(Guid orderId, string outOfStock)
    {
        DBContext context = new DBContext();
        context = context ?? new DBContext();
        // Giải mã chuỗi JSON từ query
        var orderdetail = context.TbOrderDetails
    .Where(c => c.OrderId == orderId)
    .ToList();
            var ids = outOfStock.Split(',').Select(Guid.Parse).ToList();

        // Lọc các bản ghi cần xóa
        var toRemove = orderdetail.Where(o => ids.Contains(o.ProductId)).ToList();

        // Xóa khỏi DbSet
        context.TbOrderDetails.RemoveRange(toRemove);

        // Lấy orderId (giả sử tất cả bản ghi cùng orderId)
        var orderIdd = toRemove.First().OrderId;

        var remainingOrderDetails = context.TbOrderDetails
            .Where(od => od.OrderId == orderId && !ids.Contains(od.ProductId))
            .ToList();
        decimal newTotal = 0;
        foreach (var item in remainingOrderDetails)
        {
            var price = context.TbProductDetails.Where(c=>c.Id == item.ProductId).Select(c=>c.Price).FirstOrDefault();
            var productdetail = context.TbProductDetails.Where(c => c.Id == item.ProductId).FirstOrDefault();
            productdetail.Quantity -= item.Quantity;
            newTotal += item.Quantity * price;
        }

        var order = context.TbOrders.Where(c => c.Id == orderId).FirstOrDefault();
        if (order != null)
        {
            order.TotalAmount = newTotal;
            order.Status = 4; // Cập nhật trạng thái
            
        }
        // Lưu thay đổi
        context.SaveChanges();
        return View(model: order.OrderCode);
    }
    [HttpGet]
    [Route("{id}/view")]
    public async Task<IActionResult> ViewOrder([FromRoute] string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);

        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");

        if (!rawResponse.IsSuccessStatusCode)
        {
            var errorText = await rawResponse.Content.ReadAsStringAsync();
            return StatusCode((int)rawResponse.StatusCode, new
            {
                success = false,
                message = $"Không thể lấy đơn hàng. Lỗi từ API: {errorText}"
            });
        }

        var response =
            JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                await rawResponse.Content.ReadAsStringAsync());

        var order = response!.Data;

        order.ShippingInfo.IsCustomerTakeYourSelf = order.IsCustomerTakeYourSelf;
        order.ShippingInfo.IsSameAsCustomerAddress = order.IsSameAsCustomerAddress;
        order.PaymentInfo.Products = order.Items.Select(e =>
            $"{e.ProductName} - {e.Price.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))}").ToArray();

        HttpContext.Session.SaveCurrentOrder(order);

        var tempOrderSaveButton = string.Empty;
        if (order.IsDraft || order.Code.StartsWith("TEMP"))
            tempOrderSaveButton = await RenderViewAsync(TempSaveOrderButtonPartialView, default);
        var mappedItems = order.Items.Select(x => new GUI.Models.DTOs.Order_DTO.OrderItem
        {
            ProductName = x.ProductName,
            ProductImage = x.ProductImage,
            Color = x.Color,
            Size = x.Size,
            Quantity = x.Quantity,
            Price = x.Price,
            Code = x.Code,
            Id = x.Id
        }).ToList();
        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, mappedItems),
            Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
            Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
            Buttons = await RenderViewAsync(OrderButtonActionPartialView, 1),
            IsDraft = order.IsDraft || order.Code.StartsWith("TEMP"),
            TempSaveButton = tempOrderSaveButton,
            order.ShippingInfo.IsCustomerTakeYourSelf,
            order.Status
        });
    }

    [HttpGet]
    [Route("create/online")]
    public async Task<IActionResult> CreateOnline()
    {
        var orders = await FetchOrderList();

        HttpContext.Session.GetCurrentOrder(clearFirst: true);
        ViewData["Orders"] = orders;

        return View("CreateOnline");
    }

    [HttpGet]
    [Route("create/instore")]
    public async Task<IActionResult> CreateInStore()
    {
        var orders = await FetchOrderList();

        HttpContext.Session.GetCurrentOrder(clearFirst: true);
        ViewData["Orders"] = orders;

        return View("Create");
    }

    [HttpPost]
    [Route("save-to-session")]
    public async Task<IActionResult> SaveOrder([FromBody] Checkout checkout)
    {
        var order = HttpContext.Session.GetCurrentOrder();
        if (order.Items.Count == 0)
        {
            order.Items = checkout.OrderItems;
            order.PaymentInfo.TotalAmount = order.Items.Sum(c => c.Price);
        }
        if (order.Items.Count < checkout.OrderItems.Count)
        {
            order.Items = checkout.OrderItems;
        }
        if (order.Customer.Id == Guid.Empty)
            order.Customer = checkout.CustomerInfo;

        if (string.IsNullOrEmpty(checkout.CustomerInfo.Name))
            order.Customer.Name = "Khách vãng lai";

        if (!checkout.IsShippingAddressSameAsCustomerAddress)
            order.ShippingInfo = checkout.ShippingInfo;

        order.IsDraft = checkout.IsDraft;
        order.TempOrderCreatedTime = DateTime.Now;
        order.IsCustomerTakeYourSelf = checkout.IsCustomerTakeYourSelf;
        order.IsSameAsCustomerAddress = checkout.IsShippingAddressSameAsCustomerAddress;
        order.Status = checkout.Status;

        if (order.Id == Guid.Empty)
        {
            if (order.IsCustomerTakeYourSelf)
                order.Status = 7; // set status to complete
            else
                order.Status = 1; // set status to prepare
        }

        order.PaymentInfo.ShippingFee = order.IsCustomerTakeYourSelf ? 0 : 0;

        // submit to database
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var payload = new
        {
            order.Id,
            order.Customer,
            order.Items,
            checkout.IsCustomerTakeYourSelf,
            checkout.IsDraft,
            checkout.IsShippingAddressSameAsCustomerAddress,
            order.Status,
            Shipping = order.ShippingInfo,
            Payment = order.PaymentInfo,
        };
        var updateRequest = new GUI.Models.DTOs.Order_DTO.UpdateItemOrderRequest()
        {
            Items = payload.Items,
            Status = checkout.Status
        };
        HttpResponseMessage rawResponse = order.Id != Guid.Empty
            ? await httpClient.PatchAsJsonAsync($"api/orders/update/{order.Id}", updateRequest)
            : await httpClient.PostAsJsonAsync("api/orders/create", payload);

        if (rawResponse.IsSuccessStatusCode)
        {          

            var orders = await FetchOrderList();

            try
            {
                if (order.Customer.Email != "")
                {
                    await _emailService.SendOrderConfirmationAsync(order.Customer.Email, order.Code, order.Customer.Name, order.Customer.PhoneNumber, order.Status == 7 ? "Hoàn thành" : order.StatusText, "", 1, null, order.Items, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email xác nhận: {ex.Message}");
            }

            return Json(new
            {
                Orders = await RenderViewAsync(OrderListPartialView, orders),
                Buttons = await RenderViewAsync(OrderButtonActionPartialView, 0)
            });
        }
        if (rawResponse.IsSuccessStatusCode != true)
        {
            var content = rawResponse.Content != null ? await rawResponse.Content.ReadFromJsonAsync<ResponseModel>() : null;
            if (content != null && !string.IsNullOrEmpty(content.Message))
            {
                // Trả JSON có thông báo ra giao diện và DỪNG lại
                return Json(new
                {
                    Message = content.Message
                });
            }
        }

        return BadRequest();
    }
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
    [HttpDelete]
    [Route("draft/{id}/remove")]
    public async Task<IActionResult> RemoveDraftOrder([FromRoute] string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        await httpClient.DeleteAsync($"api/orders/draft/{id}");

        var orders = await FetchOrderList();

        return Json(new
        {
            Orders = await RenderViewAsync(OrderListPartialView, orders)
        });
    }

    [HttpPost]
    [Route("add-item")]
    public async Task<IActionResult> AddItemToOrder([FromBody] OrderItem item)
    {
        var order = HttpContext.Session.GetCurrentOrder();
        var stock = await GetProductStock(item.Id);
        //if (stock.Quantity < item.Quantity)
            //return BadRequest();

        var existItem = order.Items.FirstOrDefault(e => e.Id == item.Id);
        if (existItem is null)
            order.Items.Add(item);
        else
            existItem.Quantity += 1;

        order.ReCalculatePaymentInfo();
        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }

    [HttpPatch]
    [Route("items/{id}")]
    public async Task<IActionResult> UpdateItem([FromRoute] string id, [FromQuery] int quantity)
    {
        var stock = await GetProductStock(Guid.Parse(id));
        if (stock.Quantity < quantity)
            return BadRequest();

        var order = HttpContext.Session.GetCurrentOrder();
        var existItem = order.Items.FirstOrDefault(e => e.Id == Guid.Parse(id));
        if (existItem is null)
            return BadRequest();

        existItem.Quantity = quantity;
        order.ReCalculatePaymentInfo();

        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }

    [HttpDelete]
    [Route("items/{id}")]
    public async Task<IActionResult> RemoveItemFromOrder([FromRoute] string id)
    {
        var order = HttpContext.Session.GetCurrentOrder();

        var item = order.Items.FirstOrDefault(e => e.Id == Guid.Parse(id));
        if (item is null)
            return BadRequest();

        order.Items.Remove(item);
        order.ReCalculatePaymentInfo();
        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }

    [HttpGet]
    [Route("customers/{phone}")]
    public async Task<IActionResult> SearchCustomer([FromRoute] string phone)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/customers/{phone}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<CustomerInfo>>(
                await rawResponse.Content.ReadAsStringAsync());

        var order = HttpContext.Session.GetCurrentOrder();
        order.Customer = response!.Data;

        if(order.Customer.Id != Guid.Empty)
        {
            var voucher = await CheckCustomerCanUseVoucher(order.Voucher.Id.ToString(), order.Customer.PhoneNumber);

            if (voucher is not null)
            {
                order.Voucher = voucher;
                order.PaymentInfo.VoucherId = order.Voucher.Id;
                order.PaymentInfo.VoucherCode = order.Voucher.Code;
            } else
            {
                order.Voucher = new VoucherDTO();
                order.PaymentInfo.VoucherId = order.Voucher.Id;
                order.PaymentInfo.VoucherCode = order.Voucher.Code;
            }
        }

        order.ReCalculatePaymentInfo();
        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Found = order.Customer.Id != Guid.Empty,
            Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }


    [HttpDelete]
    [Route("clear")]
    public async Task<IActionResult> ClearOrder()
    {
        var order = HttpContext.Session.GetCurrentOrder(clearFirst: true);
        var mappedItems = order.Items.Select(x => new GUI.Models.DTOs.Order_DTO.OrderItem
        {
            ProductName = x.ProductName,
            ProductImage = x.ProductImage,
            Color = x.Color,
            Size = x.Size,
            Quantity = x.Quantity,
            Price = x.Price,
            Code = x.Code,
            Id = x.Id
        }).ToList();
        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, mappedItems),
            Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
            Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
            Buttons = await RenderViewAsync(OrderButtonActionPartialView, 0),
            TempSaveButton = await RenderViewAsync(TempSaveOrderButtonPartialView, false)
        });
    }

    #region Voucher

    [HttpGet]
    [Route("apply-voucher")]
    public async Task<IActionResult> ApplyCoupon([FromQuery] string id)
    {
        var order = HttpContext.Session.GetCurrentOrder();
        var target = order.Customer.Id == Guid.Empty ? order.Customer.PhoneNumber : order.Customer.Id.ToString();

        var voucher = await CheckCustomerCanUseVoucher(id, target);

        if (voucher is null)
        {
            return BadRequest(new { Message = "Mã Voucher không hợp lệ hoặc đơn hàng không đủ điều kiện tối thiểu." });
        }

        order.Voucher = voucher;
        order.PaymentInfo.VoucherId = voucher.Id;
        order.PaymentInfo.VoucherCode = voucher.Code;

        order.ReCalculatePaymentInfo();
        Console.WriteLine($"TotalDiscount after apply: {order.PaymentInfo.TotalDiscount}");
        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }

    [HttpPost]
    [Route("cancel-apply-voucher")]
    public async Task<IActionResult> CancelCurrentAppliedVoucher()
    {
        var order = HttpContext.Session.GetCurrentOrder();

        order.Voucher = new VoucherDTO();
        order.PaymentInfo.VoucherId = Guid.Empty;
        order.PaymentInfo.VoucherCode = string.Empty;

        order.ReCalculatePaymentInfo();

        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
        });
    }

    private async Task<VoucherDTO?> CheckCustomerCanUseVoucher(string id, string target = "")
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.PostAsync($"/api/vouchers/{id}/apply?target={target}", null);

        if (rawResponse.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var voucher = JsonConvert.DeserializeObject<VoucherDTO>(await rawResponse.Content.ReadAsStringAsync());
        if (voucher == null)
            return null;

        var order = HttpContext.Session.GetCurrentOrder();
        if (order.PaymentInfo.TotalAmount < voucher.Condition)
        {
            return null; 
        }

        return voucher;
    }
    #endregion

    #region Online payment

    [HttpGet]
    [Route("payments")]
    public async Task<IActionResult> ChangePaymentMethod([FromQuery] int method)
    {
        return Ok(new
        {
            Buttons = await RenderViewAsync(OrderButtonActionPartialView, method)
        });
    }

    [HttpPost]
    [Route("payments/vnpay")]
    public async Task<IActionResult> GetVnpayCheckoutUrl([FromServices] VNPayService vnpay, [FromBody] Checkout checkout)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var order = HttpContext.Session.GetCurrentOrder();

        if (order.Customer.Id == Guid.Empty)
            order.Customer = checkout.CustomerInfo;

        if (string.IsNullOrEmpty(checkout.CustomerInfo.Name))
            order.Customer.Name = "Khách vãng lai";

        if (!checkout.IsShippingAddressSameAsCustomerAddress)
            order.ShippingInfo = checkout.ShippingInfo;

        order.IsDraft = checkout.IsDraft;
        order.TempOrderCreatedTime = DateTime.Now;
        order.IsCustomerTakeYourSelf = checkout.IsCustomerTakeYourSelf;
        order.IsSameAsCustomerAddress = checkout.IsShippingAddressSameAsCustomerAddress;
        order.Status = checkout.Status;

        if (order.Id == Guid.Empty)
        {
            if (order.IsCustomerTakeYourSelf)
                order.Status = 7; // set status to complete
            else
                order.Status = 1; // set status to prepare
        }

        order.PaymentMethod = 2;
        order.PaymentStatus = 0; // set status payment to wait

		var payload = new
		{
			order.Id,
			order.Customer,
			order.Items,
			order.IsCustomerTakeYourSelf,
			order.IsDraft,
			order.IsSameAsCustomerAddress,
			order.Status,
			Shipping = order.ShippingInfo,
			Payment = order.PaymentInfo,
			order.PaymentMethod
		};

		using var httpClient = new HttpClient();
		httpClient.BaseAddress = new Uri(URI);

		HttpResponseMessage rawResponse = order.Id != Guid.Empty
		? await httpClient.PatchAsJsonAsync($"api/orders/{order.Id}", payload)
		: await httpClient.PostAsJsonAsync("api/orders/create", payload);

		HttpContext.Session.SaveCurrentOrder(order);

        var paymentUrl = vnpay.SendRequest(ip, order.Code, Convert.ToInt32(order.PaymentInfo.FinalAmount * 100));
		if (rawResponse.IsSuccessStatusCode)
		{
			return Ok(new
        {
            Url = paymentUrl
        });
        }
        else
        {
            return Ok(new
            {
				Url = "http://localhost:5011/orders/create"
			});
        }
	}

    [HttpGet]
    [Route("payments/vnpay/success")]
    public async Task<IActionResult> OnlineCheckoutSuccess(
        [FromServices] IHubContext<OrderHub> hub,
        [FromQuery] string vnp_ResponseCode)
    {
        if (vnp_ResponseCode == "00")
        {
            var order = HttpContext.Session.GetCurrentOrder();

            //var payload = new
            //{
            //    order.Id,
            //    order.Customer,
            //    order.Items,
            //    order.IsCustomerTakeYourSelf,
            //    order.IsDraft,
            //    order.IsSameAsCustomerAddress,
            //    order.Status,
            //    Shipping = order.ShippingInfo,
            //    Payment = order.PaymentInfo,
            //    order.PaymentMethod
            //};

            //using var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(URI);

            //HttpResponseMessage rawResponse = order.Id != Guid.Empty
            //? await httpClient.PatchAsJsonAsync($"api/orders/{order.Id}", payload)
            //: await httpClient.PostAsJsonAsync("api/orders/create", payload);

            //if (rawResponse.IsSuccessStatusCode)
            //{
                var orders = await FetchOrderList();
                order = HttpContext.Session.GetCurrentOrder(clearFirst: true);

                var message = JsonConvert.SerializeObject(new
                {
                    Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
                    Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
                    Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
                    Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
                    Buttons = await RenderViewAsync(OrderButtonActionPartialView, 0),
                    TempSaveButton = await RenderViewAsync(TempSaveOrderButtonPartialView, false)
                });

                await hub.Clients.All.SendAsync("PaymentSuccess", message);

                return View();
            //}
        }

        await hub.Clients.All.SendAsync("PaymentFail", "Thanh toán thất bại");


        return Ok();
    }

    #endregion

    #region Option Change

    [HttpGet]
    [Route("shipping")]
    public async Task<IActionResult> ChangeShipping([FromQuery] int method)
    {
        var order = HttpContext.Session.GetCurrentOrder();
        if(method == 0)
        {
            order.IsCustomerTakeYourSelf = true;
            order.PaymentInfo.IsCustomerTakeYourSelf = true;
            order.PaymentInfo.ShippingFee = 0;
        } else
        {
            order.IsCustomerTakeYourSelf = false;
            order.PaymentInfo.IsCustomerTakeYourSelf = false;
            order.PaymentInfo.ShippingFee = 0;
            order.Status = 1;
        }

        order.ReCalculatePaymentInfo();
        HttpContext.Session.SaveCurrentOrder(order);

        return Ok(new
        {
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo)
        });
    }

    #endregion

    private async Task<string> RenderViewAsync(string viewName, object? model)
    {

        ViewData.Model = model;

        using var writer = new StringWriter();
        IViewEngine viewEngine = HttpContext.RequestServices.GetService<ICompositeViewEngine>()!;
        ViewEngineResult viewResult = viewEngine!.FindView(ControllerContext, viewName, false);

        if (viewResult.Success == false)
        {
            return $"A view with the name {viewName} could not be found";
        }

        ViewContext viewContext = new(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return writer.GetStringBuilder().ToString();
    }

    private static async Task<IEnumerable<OrderDetail>> FetchOrderList()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync("/api/admin/orders/not-completed");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<IEnumerable<OrderDetail>>>(
                await rawResponse.Content.ReadAsStringAsync());

        var data = response!.Data;   
        //foreach (var order in data)
        //    order.ReCalculatePaymentInfo();

        return data;
    }

    private static async Task<Stock> GetProductStock(Guid id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var response = await httpClient.GetAsync($"api/products/{id}/stock");

        return JsonConvert.DeserializeObject<Stock>(await response.Content.ReadAsStringAsync())!;
    }
    [HttpGet]
    [Route("{id}/export-pdf")]
    public async Task<IActionResult> ExportInvoicePdf(string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
        var response = JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(await rawResponse.Content.ReadAsStringAsync());

        if (response == null || response.Data == null)
        {
            return NotFound("Không tìm thấy hóa đơn.");
        }

        var order = response.Data;
        order.ReCalculatePaymentInfo();
        if (order.Status != 7) 
        {
            return BadRequest("Chỉ có thể tải PDF cho hóa đơn ở trạng thái 'Hoàn thành'.");
        }
        return new ViewAsPdf("Invoice", order)
        {
            FileName = $"Invoice_{order.Code}.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.Letter, 
            PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 15, 20, 15),
            CustomSwitches = "--print-media-type --no-stop-slow-scripts --encoding UTF-8"
        };
    }
}