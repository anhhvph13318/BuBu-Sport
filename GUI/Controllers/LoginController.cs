using GUI.Controllers.Shared;
using GUI.Models;
using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Login;
using GUI.Shared;
using Newtonsoft.Json;

namespace GUI.Controllers
{
    public class LoginController : ControllerSharedBase
    {
        private readonly ILogger<LoginController> _logger;
        private HttpService httpService;
        public LoginController(ILogger<LoginController> logger, IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
            httpService = new();
        }

        [Route("/SignIn")]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var URL = _settings.APIAddress + "api/Login/check-login";
            var param = JsonConvert.SerializeObject(request);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(res) ?? new();
            if (result.Status == "200")
            {
                var userId = result.Messages?.FirstOrDefault().MessageText;
                HttpContext.Session.SetString("CurrentUserId",userId);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.SweetAlertShowMessage = SweetAlertHelper.ShowMessage("Thông báo",
                    result.Messages?.FirstOrDefault().MessageText, SweetAlertMessageType.error);
                return View();
                
            }
           
        }
        [HttpPost]
        public  IActionResult ForgotPassWord(string Email)
        {
            ViewBag.SweetAlertShowMessage = SweetAlertHelper.ShowMessage("Thông báo",
                $"Mật khẩu đã được gửi về tài khoản {Email} của Anh/chị! Vui lòng Anh/chị kiểm tra lại mật khẩu gửi về mail và đăng nhập lại hệ thống.", SweetAlertMessageType.success);
            return View("Login");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}