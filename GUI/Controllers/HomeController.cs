using DATN_ACV_DEV.Model_DTO.Account_DTO;
using GUI.Controllers.Shared;
using GUI.FileBase;
using GUI.Models;
using GUI.Shared;
using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GUI.Controllers
{
    public class HomeController : ControllerSharedBase
    {
        private readonly ILogger<HomeController> _logger;
		private HttpService httpService;

		public HomeController(ILogger<HomeController> logger, IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
			httpService = new();

		}

		public IActionResult Index()
        {
            return View(_settings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<ActionResult> SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SignUp(CreatedAccountRequest obj, string rePassword)
        {
			try
			{
                if (obj.Password == rePassword)
                {
                    var URL = _settings.APIAddress + "api/CreateAccount/Process";
					var param = JsonConvert.SerializeObject(obj);
					var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
					var result = JsonConvert.DeserializeObject<BaseResponse<GetListAccountResponse>>(res) ?? new();
					if (result.Status == "400")
					{
						ModelState.AddModelError("UserName", result.Messages.FirstOrDefault().MessageText);
						return Empty;
					}
					return RedirectToAction(nameof(Index));
                }
                return View();
			}
			catch
			{
				return View();
			}
		}
    }
}
