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
            string email = "nguyenquan14124@gmail.com";
            string orderCode = "PH38284";
            string customerName = "Nguyễn Minh Quân";
            decimal totalAmount = 5600000;
            //await _emailService.SendOrderConfirmationAsync(email, orderCode, customerName, totalAmount);
            return Ok("Email đã được gửi thành công.");

        }
    }
}
