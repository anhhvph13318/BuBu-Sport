using GUI.FileBase;
using GUI.Models.DTOs.Order_DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GUI.Controllers;

[Controller]
[Route("orders")]
public class OrderController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? code = "", string? customerName = "", int status = 0)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:44383");
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders?code={code}&customerName={customerName}&status={status}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<IEnumerable<OrderListItem>>>(
                await rawResponse.Content.ReadAsStringAsync());
        
        return View(response.Data);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Detail(string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:44383");
        var rawResponse = await httpClient.GetAsync($"/api/admin/orders/{id}");
        var response =
            JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(
                await rawResponse.Content.ReadAsStringAsync());
        
        return View(response.Data);
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }
}