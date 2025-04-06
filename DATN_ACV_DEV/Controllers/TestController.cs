using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public class SendEmailRequest
        {
            public string Email { get; set; }
            public string OrderCode { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalAmount { get; set; }
        }
        public class NewPasswordRequest
        {
            public string email { get; set; }
        }
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            await _emailService.SendOrderConfirmationAsync(request.Email, request.OrderCode, request.CustomerName, request.TotalAmount);
            return Ok("Email đã được gửi thành công.");

        }
        [HttpPost("send-new-password")]
        public async Task<IActionResult> SendNewPassword([FromBody] NewPasswordRequest request)
        {
            await _emailService.SendNewPasswordAsync(request.email);
            return Ok("Email đã được gửi thành công.");
        }

    }
}
