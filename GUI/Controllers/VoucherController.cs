using GUI.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

class VoucherController : ControllerSharedBase
{
  private const string URI = "http://localhost:5059";
  // private const string URI = "https://localhost:";
  public async Task<IActionResult> Index()
  {
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(URI);
    var rawResponse = await httpClient.GetAsync($"/api/admin/orders?code={code}&customerName={customerName}&status={status}");
    var response =
        JsonConvert.DeserializeObject<BaseResponse<IEnumerable<VoucherListItem>>>(
            await rawResponse.Content.ReadAsStringAsync());

    return View();
  }
}