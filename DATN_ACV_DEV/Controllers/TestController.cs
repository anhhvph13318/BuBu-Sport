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
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail()
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
