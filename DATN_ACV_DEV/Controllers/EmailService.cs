using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using System.Text;

namespace DATN_ACV_DEV.Controllers
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string email, string orderCode, string customerName, decimal totalAmount);
        Task SendNewPasswordAsync(string email); 
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
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = int.TryParse(configuration["Email:SmtpPort"], out int port) ? port : 587;
            _smtpUsername = configuration["Email:Username"];
            _smtpPassword = configuration["Email:Password"];
            _fromEmail = configuration["Email:FromEmail"];
            _fromName = configuration["Email:FromName"];
        }

        public async Task SendOrderConfirmationAsync(string email, string orderCode, string customerName, decimal totalAmount)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(customerName, email));
            message.Subject = $"Xác nhận đơn hàng #{orderCode}";
                
            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
                <p>Chào {customerName},</p>
                <p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi.</p>
                <p>Mã đơn hàng: <strong>{orderCode}</strong></p>
                <p>Tổng số tiền: <strong>{totalAmount:C}</strong></p>
                <p>Trân trọng</p>";

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendNewPasswordAsync(string email)
        {

            string newPassword = GenerateRandomPassword(); 

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Yêu cầu đặt lại mật khẩu website BuBuSport";

            var builder = new BodyBuilder();
            builder.HtmlBody = 
            $@"
                <h2>Yêu cầu đặt lại mật khẩu</h2>
                <p>Xin chào</p>
                <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn.</p>
                <p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p>
            ";

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