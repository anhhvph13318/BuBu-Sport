using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using GUI.Models.DTOs.Voucher_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GUI.Controllers;

[Controller]
[Route("vouchers")]
[Authorize(Roles = "Admin")]
public class VoucherController : Controller
{
    private const string URI = "http://localhost:5059";
    //private const string URI = "https://localhost:44383";

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] string code,
        [FromQuery] string name,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] VoucherUnit? unit)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/vouchers?code={code}&name={name}&startDate={startDate}&endDate={endDate}&unit={unit}");
            
        if(rawResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Have error with api call");
        }

        var response =
            JsonConvert.DeserializeObject<BaseResponse<GetListVoucherResponse>>(
                await rawResponse.Content.ReadAsStringAsync());

        return View(response!.Data.LstVoucher);
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        var model = new VoucherFormModel
        {
            IsEditMode = false,
            Voucher = new VoucherDTO()
        };
        return View(model);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Detail([FromRoute] string id)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);
        var rawResponse = await httpClient.GetAsync($"/api/vouchers/{id}");

        if (rawResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Have error with api call");
        }

        var response =
            JsonConvert.DeserializeObject<BaseResponse<VoucherDTO>>(
                await rawResponse.Content.ReadAsStringAsync());

        var model = new VoucherFormModel
        {
            IsEditMode = true,
            Voucher = response!.Data
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Handle(VoucherFormModel model)
    {
        var validPeriod = model.Voucher.EndDate < model.Voucher.StartDate;
        var validStartDate = model.Voucher.StartDate < DateTime.Now;
        var validDiscount = model.Voucher.Unit == VoucherUnit.Percent && model.Voucher.Discount > 80;
        if (!TryValidateModel(model.Voucher) || validPeriod || validDiscount || validStartDate)
        {
            if(validPeriod)
                ModelState.AddModelError("Voucher.EndDate", "Ngày kết thúc không thể nhỏ hơn ngày bắt đầu");
            if(validStartDate)
                ModelState.AddModelError("Voucher.StartDate", "Ngày bắt đầu không thể nhỏ hơn ngày hiện tại");
            if (validDiscount)
                ModelState.AddModelError("Voucher.Discount", "Giá trị voucher không thể vượt quá 80%");

            return model.IsEditMode
                ? View("Detail", model)
                : View("Create", model);
        }

        if (model.Voucher.Unit == VoucherUnit.Money)
            model.Voucher.MaxDiscount = model.Voucher.Discount;


        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URI);

        var response = model.IsEditMode
            ? await httpClient.PatchAsJsonAsync($"/api/vouchers/{model.Voucher.Id}", model.Voucher)
            : await httpClient.PostAsJsonAsync("/api/vouchers", model.Voucher);

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError("Voucher.Code", "Mã voucher đã tồn tại");
            return model.IsEditMode
                ? View("Detail", model)
                : View("Create", model);
        }

        throw new HttpRequestException("Have error when call api");
    }
}