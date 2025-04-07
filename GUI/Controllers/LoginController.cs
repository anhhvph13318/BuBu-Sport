using GUI.Controllers.Shared;
using GUI.Models;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using GUI.FileBase;
using DATN_ACV_DEV.Model_DTO.Login;
using GUI.Shared;
using Newtonsoft.Json;
using GUI.Models.DTOs.Login_DTO;
using GUI.Model_DTO.User_DTO;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using DATN_ACV_DEV.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DATN_ACV_DEV.Controllers;
using GUI.Models.DTOs.ResetPassWord_DTO;

namespace GUI.Controllers
{
    public class LoginController : ControllerSharedBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<LoginController> _logger;
        private HttpService httpService;

        public LoginController(ILogger<LoginController> logger, IOptions<CommonSettings> settings, IEmailService emailService)
        {
            _emailService = emailService;
            _settings = settings.Value;
            _logger = logger;
            httpService = new();
        }

        [Route("/SignIn")]
        public async Task<IActionResult> Login([FromQuery] int? action)
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync();

            ViewBag.Action = action;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginNow(LoginRequest request)
        {
            var URL = _settings.APIAddress + "api/Login/check-login";
            var param = JsonConvert.SerializeObject(request);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(res) ?? new();
            if (result.Status == "200")
            {
                
                var userId = result.Messages!.First().MessageText;
                
                var role = result.Data.Role == 0 ? "Guest" : "Admin";
                var claims = new List<Claim>
                {
                   new(ClaimTypes.NameIdentifier, userId),
                   new(ClaimTypes.Role, role)
                };
                var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                    principal: new ClaimsPrincipal(claimIdentity));

                HttpContext.Session.SetString("CurrentUserId", userId);
                //TempData["SweetAlertMessage"] = Alert.SweetAlertHelper.ShowSuccess("Thành công!", "Đăng nhập thành công.");

                return result.Data != null && result.Data.Role == 0
                    ? RedirectToAction("Store", "Storefront")
                    : RedirectToAction("", "Product");
            }
            else
            {
                ViewBag.SweetAlertShowMessage = SweetAlertHelper.ShowMessage("Thông báo",
                    result.Messages?.FirstOrDefault().MessageText, SweetAlertMessageType.error);
                TempData["SweetAlertMessage"] = Alert.SweetAlertHelper.ShowError("Thất bại!", "Đăng nhập thất bại.");
                return RedirectToAction("Login");

            }

        }
        [HttpPost]
        public async Task<IActionResult> Register(CreatedAccountRequest request)
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(10, 100); // Tạo số ngẫu nhiên từ 10 đến 99
                request.Role = 0;
                request.Name = request.PhoneNumber;
                var URL = _settings.APIAddress + "api/CreateAccount/Process";
                var param = JsonConvert.SerializeObject(request);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListUserResponse>>(res) ?? new();
                if (result.Status == "200")
                {
                    return RedirectToAction("Login");
                }
                if (result.Status == "400")
                {
                    ModelState.AddModelError("UserName", result.Messages.FirstOrDefault().MessageText);
                    return RedirectToAction("Login");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassWord(string Email)
        {
            try
            {
                ViewBag.SweetAlertShowMessage = SweetAlertHelper.ShowMessage("Thông báo",
                $"Mật khẩu đã được gửi về tài khoản {Email} của Anh/chị! Vui lòng Anh/chị kiểm tra lại mật khẩu gửi về mail và đăng nhập lại hệ thống.",
                SweetAlertMessageType.success);
                var URL = _settings.APIAddress + "api/CreateContentEmail/Process";
                var param = JsonConvert.SerializeObject(Email);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<ContentEmailRespone>>(res) ?? new();
                await _emailService.SendOrderConfirmationAsync(Email,"",result.Data.customerName,result.Data.phonenumber,"",result.Data.password,0);
                TempData["Message"] = "Email đã được gửi thành công.";

                // Chuyển hướng về trang Login
                return Redirect("http://localhost:5011/SignIn");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có (ví dụ log lỗi hoặc thông báo)
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi email.";

                // Chuyển hướng về trang Login với thông báo lỗi
                return Redirect("http://localhost:5011/SignIn");
            }
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}