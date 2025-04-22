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
using DATN_ACV_DEV.Model_DTO.SendEmail_DTO;
using DATN_ACV_DEV.Model_DTO.Order_DTO;

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
        [HttpPost]
        public async Task<IActionResult> UpdateStatusONOrder([FromBody] UpdatStatusOrderRequest request)
        {
            var orderUrl = _settings.APIAddress + $"api/admin/orders/{request.id}";
            var orderResponse = await httpService.GetAsync(orderUrl);
            var order = JsonConvert.DeserializeObject<BaseResponse<OrderDetail>>(orderResponse)?.Data;

            if (order != null && order.Status == 3)
            {
                return BadRequest(new { success = false, message = "Không thể cập nhật trạng thái cho đơn hàng đã hủy." });
            }

            request.statusText = request.status == 1 ? "Chuẩn bị hàng" : (request.status == 2 ? "Đang vận chuyển" : "Hoàn thành");

            var URL = _settings.APIAddress + "api/UpdateStatusONOrder/Process";
            var param = JsonConvert.SerializeObject(request);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(res) ?? new();

            await _emailService.SendOrderConfirmationAsync(request.email, request.code, request.name, request.phone, request.statusText, "", 1,null, request.products);

            return Redirect($"/orders/{request.id}");
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
                    TempData["RegisterSuccess"] = "Đăng ký thành công!";
                    return RedirectToAction("Login");
                }
                if (result.Status == "400")
                {
                    TempData["RegisterError"] = result.Messages.FirstOrDefault().MessageText;
                    return RedirectToAction("Login");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Login));
            }
        }
        public static string GenerateRandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassWord(CreateEmailRequest request)
        {
            try
            {
                var password = GenerateRandomString();
                //ViewBag.SweetAlertShowMessage = SweetAlertHelper.ShowMessage("Thông báo",
                //$"Mật khẩu đã được gửi về tài khoản {request.Email} của Anh/chị! Vui lòng Anh/chị kiểm tra lại mật khẩu gửi về mail và đăng nhập lại hệ thống.",
                //SweetAlertMessageType.success);
                request.Emailtype = 0;
                request.password = password;
                var URL = _settings.APIAddress + "api/CreateContentEmail/Process";
                var param = JsonConvert.SerializeObject(request);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<ContentEmailRespone>>(res) ?? new();
                if (result != null && result.Messages.Count == 0 && result.Data.customerName != null)
                {
                    await _emailService.SendOrderConfirmationAsync(request.Email, "", result.Data.customerName, result.Data.phonenumber, "", result.Data.password, 0, null, null);
                    TempData["Message"] = "Mật khẩu đã được gửi về tài khoản " + request.Email + " vui lòng kiểm tra lại mật khẩu gửi về email và đăng nhập lại hệ thống.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Email chưa được đăng ký tài khoản ở BuBu Sport, vui lòng kiểm tra lại !!!";
                }



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