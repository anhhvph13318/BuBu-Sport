using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using System.Threading.Tasks;
using System.Text;

namespace DATN_ACV_DEV.Controllers
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type);
    }
    public class EmailService : IEmailService
    {
        
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _smtpUsername = "sprtbubu@gmail.com";
            _smtpPassword = "luys adyn vknr bdtp";
            _fromEmail = "sprtbubu@gmail.com";
            _fromName = "BuBuSport";
        }

        public async Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BuBuSport", "sprtbubu@gmail.com"));
            message.To.Add(new MailboxAddress(customerName, email));
            message.Subject = type == 0 ? $"Yêu cầu cấp lại mật khẩu BuBu Sport" : ( type == 2 ? $"Xác nhận hủy đơn hàng #{orderCode}" : $"Cập nhật trạng thái đơn hàng #{orderCode}");
            var resetpass = $@"
                <h2>Yêu cầu cấp lại mật khẩu BuBu Sport</h2>
                <p>Xin chào {customerName},</p>
                <p><strong>Số điện thoại:</strong> {phonenumber}</p>
                <p>Chúng tôi đã nhận được yêu cầu cấp lại mật khẩu của bạn.</p>
                <p>Mật khẩu mới của bạn là {password:N0}</p>
                <p>Nếu bạn không thực hiên yêu cầu này vui lòng bỏ qua email này và cài đặt lại mật khẩu.</p>
                <p>để đảm bảo tính bảo mật cho tài khoản của bạn</p>
                <p>Xin cảm ơn,</p>
                <p>Nhóm tài khoản BuBu Sport</p>
            ";
            var orderstatus = $@"
                <h2>Trạng thái đơn hàng BuBu Sport</h2>
                <p>Xin chào {customerName},</p>
                <p><strong>Số điện thoại:</strong> {phonenumber}</p>
                <p><strong>Mã hóa đơn:</strong> {orderCode}</p>
                <p>
                  Chúng tôi xin thông báo đơn hàng của bạn đã được cập nhật trạng thái 
                  <strong style=""color: #d63384; background-color: #fce4ec; padding: 2px 6px; border-radius: 4px;"">
                    {status}
                  </strong>
                </p>
                <p>Cảm ơn bạn đã tin tưởng sử dụng dịch vụ của chúng tôi !!!</p>
                <p>Xin cảm ơn,</p>
                <p>Nhóm tài khoản BuBu Sport</p>
            ";
            var cancelorder = $@"
                <h2>Xác thực hủy đơn hàng BuBu Sport</h2>
                <p>Xin chào {customerName},</p>
                <p><strong>Số điện thoại:</strong> {phonenumber}</p>
                <p><strong>Mã hóa đơn:</strong> {orderCode}</p>
                <p>
                  Chúng tôi xin thông báo đơn hàng của bạn đã được yêu cầu cập nhật trạng thái
                  <strong style=""color: #d63384; background-color: #fce4ec; padding: 2px 6px; border-radius: 4px;"">
                    Hủy đơn hàng
                  </strong>
                </p>
                <p>Đây là mã để xác thực hành động hủy đơn hàng của bạn : {password}</p>
                <p>Nếu bạn không thực hiên yêu cầu này vui lòng bỏ qua email này</p>
                <p>Cảm ơn bạn đã tin tưởng sử dụng dịch vụ của chúng tôi !!!</p>
                <p>Xin cảm ơn,</p>
                <p>Nhóm tài khoản BuBu Sport</p>
            ";
            var builder = new BodyBuilder();
            builder.HtmlBody = type == 0 ? resetpass : ( type == 2 ? cancelorder : orderstatus );



            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


        public static string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}