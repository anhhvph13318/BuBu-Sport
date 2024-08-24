using Azure;
using GUI.FileBase;
using GUI.Models.DTOs.Order_DTO;
using GUI.Models.DTOs.Voucher_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Globalization;

namespace GUI.Controllers;

[Controller]
[Route("orders")]
public class OrderController : Controller
{
    private const string URI = "http://localhost:5059";
    //private const string URI = "https://localhost:44383";
    private const string OrderItemListPartialView = "_OrderItemListPartialView";
    private const string OrderCustomerInfoPartialView = "_OrderCustomerInfoPartialView";
    private const string OrderPaymentInfoPartialView = "_OrderPaymentInfoPartialView";
    private const string OrderShippingInfoPartialView = "_OrderShippingInfoPartialView";
    private const string OrderButtonActionPartialView = "_OrderButtonActionPartialView";
    private const string OrderTempListPartialView = "_OrderTempListPartialView";
    private const string AvailableVoucherPartialView = "_AvailableVoucherPartialView";

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

        return View(response!.Data);
    }

    [HttpGet]
    [Route("{id}/view")]
    public async Task<IActionResult> ViewOrder([FromRoute] string id)
    {
        var order = HttpContext.Session.GetOrderFromList(id);
        var isTempOrder = true;
        if (order is null)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URI);
            var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
            var response =
                JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                    await rawResponse.Content.ReadAsStringAsync());

            order = response!.Data;
            isTempOrder = false;
        }

        order.ShippingInfo.IsCustomerTakeYourSelf = order.IsCustomerTakeYourSelf;
        order.ShippingInfo.IsSameAsCustomerAddress = order.IsSameAsCustomerAddress;
        order.PaymentInfo.ProductName = string.Join("\n", order.Items.Select(item => $"{item.ProductName} - {item.Price.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))} "));

        HttpContext.Session.SaveCurrentOrder(order);

        return Json(new
        {
            Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
            Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
            Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
            Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
            Buttons = await RenderViewAsync(OrderButtonActionPartialView, isTempOrder),
            order.ShippingInfo.IsCustomerTakeYourSelf,
            order.Status
        });
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/not-completed");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<IEnumerable<OrderListItem>>>(
                await rawResponse.Content.ReadAsStringAsync());

        ViewData["OrderLoadedFromDb"] = response!.Data;
        ViewData["OrderTemps"] = HttpContext.Session.GetTempOrders();

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

        order.IsDraft = true;
        order.TempOrderCreatedTime = DateTime.Now;
        order.IsCustomerTakeYourSelf = checkout.IsCustomerTakeYourSelf;
        order.IsSameAsCustomerAddress = checkout.IsShippingAddressSameAsCustomerAddress;
        var tempOrders = HttpContext.Session.SaveTempOrder(order, out bool alreadySave);

        // submit to database
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var payload = new
        {
            order.Customer,
            order.Items,
            checkout.IsCustomerTakeYourSelf,
            checkout.IsShippingAddressSameAsCustomerAddress,
            checkout.Status,
            Shipping = order.ShippingInfo,
            Payment = order.PaymentInfo,
        };
        HttpResponseMessage rawResponse = alreadySave
            ? await httpClient.PatchAsJsonAsync($"api/orders/{order.Id}", payload)
            : await httpClient.PostAsJsonAsync("api/orders/create", payload);

        if (rawResponse.IsSuccessStatusCode)
        {
            return Json(new
            {
                TempOrders = await RenderViewAsync(OrderTempListPartialView, tempOrders)
            });
        }
        return BadRequest();


    }

    [HttpDelete]
    [Route("draft/{id}/remove")]
    public async Task<IActionResult> RemoveDraftOrder([FromRoute] string id)
    {
        var orders = HttpContext.Session.RemoveTempOrder(id, out bool alreadyRemove);
        if (alreadyRemove)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URI);
            await httpClient.DeleteAsync($"api/orders/draft/{id}");
        }

        return Json(new
        {
            TempOrders = await RenderViewAsync(OrderTempListPartialView, orders)
        });
    }

    [HttpPost]
    [Route("add-item")]
    public async Task<IActionResult> AddItemToOrder([FromBody] OrderItem item)
    {
        var order = HttpContext.Session.GetCurrentOrder();

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

    [HttpPost]
    [Route("checkout")]
    public async Task<IActionResult> Checkout([FromBody] Checkout checkout)
    {
        var order = HttpContext.Session.GetCurrentOrder();
        if (order.Customer.Id == Guid.Empty)
            order.Customer = checkout.CustomerInfo;

        if (!checkout.IsShippingAddressSameAsCustomerAddress)
            order.ShippingInfo = checkout.ShippingInfo;

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var payload = new
        {
            order.Customer,
            order.Items,
            order.Code,
            checkout.IsCustomerTakeYourSelf,
            checkout.IsShippingAddressSameAsCustomerAddress,
            checkout.Status,
            Shipping = order.ShippingInfo,
            Payment = order.PaymentInfo,
        };
        var rawResponse = await httpClient.PostAsJsonAsync("api/orders/create", payload);

        if (rawResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            order = new OrderDetail
            {
                Customer = new CustomerInfo(),
                ShippingInfo = new ShippingInfo(),
                PaymentInfo = new PaymentInfo(),
                Items = new List<OrderItem>()
            };

            HttpContext.Session.SaveCurrentOrder(order);

            return Json(new
            {
                Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
                Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
                Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
                Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
                Buttons = await RenderViewAsync(OrderButtonActionPartialView, true)
            });
        }

        return BadRequest();
    }

    [HttpPatch]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] Checkout checkout)
    {
        var order = HttpContext.Session.GetCurrentOrder();

        if (!checkout.IsShippingAddressSameAsCustomerAddress)
            order.ShippingInfo = checkout.ShippingInfo;

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var payload = new
        {
            order.Id,
            checkout.IsCustomerTakeYourSelf,
            checkout.IsShippingAddressSameAsCustomerAddress,
            checkout.Status,
            Shipping = order.ShippingInfo,
            Payment = order.PaymentInfo
        };
        var rawResponse = await httpClient.PatchAsJsonAsync($"api/orders/{order.Id}", payload);

        if (rawResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return Json(new
            {
                Items = await RenderViewAsync(OrderItemListPartialView, order.Items),
                Customer = await RenderViewAsync(OrderCustomerInfoPartialView, order.Customer),
                Payment = await RenderViewAsync(OrderPaymentInfoPartialView, order.PaymentInfo),
                Shipping = await RenderViewAsync(OrderShippingInfoPartialView, order.ShippingInfo),
                Buttons = await RenderViewAsync(OrderButtonActionPartialView, false)
            });
        }

        return BadRequest();
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
            Buttons = await RenderViewAsync(OrderButtonActionPartialView, true)
        });
    }

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

    private async Task<string> RenderViewAsync(string viewName, object model)
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
}