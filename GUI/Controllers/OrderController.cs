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
using OrderItem = GUI.Models.DTOs.Order_DTO.OrderItem;

namespace GUI.Controllers;

[Controller]
[Route("orders")]
//[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
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

    [HttpGet]
    public async Task<IActionResult> Index(string? code = "", string? customerName = "", int status = 0)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders?code={code}&customerName={customerName}&status={status}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<IEnumerable<OrderListItem>>>(
                await rawResponse.Content.ReadAsStringAsync());

        return View(response!.Data);
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
    [Route("{id}")]
    public async Task<IActionResult> Detail(string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                await rawResponse.Content.ReadAsStringAsync());

        var order = response!.Data;
        order.ReCalculatePaymentInfo();

        return View(response!.Data);
    }

    [HttpGet]
    [Route("{id}/view")]
    public async Task<IActionResult> ViewOrder([FromRoute] string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                await rawResponse.Content.ReadAsStringAsync());

        var order = response!.Data;

        order.ShippingInfo.IsCustomerTakeYourSelf = order.IsCustomerTakeYourSelf;
        order.ShippingInfo.IsSameAsCustomerAddress = order.IsSameAsCustomerAddress;
        order.PaymentInfo.Products = order.Items.Select(e => $"{e.ProductName} - {e.Price.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))}").ToArray();

        HttpContext.Session.SaveCurrentOrder(order);

        var tempOrderSaveButton = string.Empty;
        if(order.IsDraft || order.Code.StartsWith("TEMP"))
            tempOrderSaveButton = await RenderViewAsync(TempSaveOrderButtonPartialView, default);

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
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
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        var onlineOrders = await FetchOrderList();

        HttpContext.Session.GetCurrentOrder(clearFirst: true);
        ViewData["Orders"] = onlineOrders;

        return View();
    }

    [HttpPost]
    [Route("save-to-session")]
    public async Task<IActionResult> SaveOrder([FromBody] Checkout checkout)
    {
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

        if(order.Id == Guid.Empty)
        {
            if (order.IsCustomerTakeYourSelf)
                order.Status = 7; // set status to complete
            else
                order.Status = 1; // set status to prepare
        }

        order.PaymentInfo.ShippingFee = order.IsCustomerTakeYourSelf ? 0 : 30000;


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
        HttpResponseMessage rawResponse = order.Id != Guid.Empty
            ? await httpClient.PatchAsJsonAsync($"api/orders/{order.Id}", payload)
            : await httpClient.PostAsJsonAsync("api/orders/create", payload);

        if(rawResponse.IsSuccessStatusCode)
        {
            var orders = await FetchOrderList();
            return Json(new 
            { 
                Orders = await RenderViewAsync(OrderListPartialView, orders),
                Buttons = await RenderViewAsync(OrderButtonActionPartialView, 0)
            });
        }

        return BadRequest();
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
        if (stock.Quantity < item.Quantity)
            return BadRequest();

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

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
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
            return BadRequest();

        order.Voucher = voucher;
        order.PaymentInfo.VoucherId = order.Voucher.Id;
        order.PaymentInfo.VoucherCode = order.Voucher.Code;

        order.ReCalculatePaymentInfo();

        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
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

    private static async Task<VoucherDTO?> CheckCustomerCanUseVoucher(string id, string target = "")
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.PostAsync($"/api/vouchers/{id}/apply?target={target}", null);

        if (rawResponse.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        return JsonConvert.DeserializeObject<VoucherDTO>(
                await rawResponse.Content.ReadAsStringAsync());
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
    public IActionResult GetVnpayCheckoutUrl([FromServices] VNPayService vnpay, [FromBody] Checkout checkout)
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

        HttpContext.Session.SaveCurrentOrder(order);

        var paymentUrl = vnpay.SendRequest(ip, order.Code, Convert.ToInt32(order.PaymentInfo.FinalAmount * 100));

        return Ok(new
        {
            Url = paymentUrl
        });
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

            if (rawResponse.IsSuccessStatusCode)
            {
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
            }
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
        } else
        {
            order.IsCustomerTakeYourSelf = false;
            order.PaymentInfo.IsCustomerTakeYourSelf = false;
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

        foreach (var order in data)
            order.ReCalculatePaymentInfo();

        return data;
    }

    private static async Task<Stock> GetProductStock(Guid id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var response = await httpClient.GetAsync($"api/products/{id}/stock");

        return JsonConvert.DeserializeObject<Stock>(await response.Content.ReadAsStringAsync())!;
    }
    
}