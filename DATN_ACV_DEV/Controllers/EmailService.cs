using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Threading.Tasks;

namespace DATN_ACV_DEV.Controllers
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string email, string orderCode, string customerName, decimal totalAmount);
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
            _smtpPort = int.Parse(configuration["Email:SmtpPort"]);
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
            builder.HtmlBody = 
            $@"
                <h2>Đơn thông báo đóng tiền học lại</h2>
                <p>Xin chào {customerName},</p>
                <p><strong>Mã sinh viên:</strong> {orderCode}</p>
                <p>Em đã không đủ điều kiện để qua môn Đồ Án Tốt Nghiệp. Em cần đóng tiền để được học lại môn vào kì tiếp theo</p>
                <p>Vui lòng chuyển khoản số tiền {totalAmount:N0}</p>
                <p>Vào số tài khoản của nhà trường : 1301102004 MBBANK</p>
                <p>Để đủ điều kiện học lại</p>
                <p><strong>Tổng tiền:</strong> {totalAmount:N0} VNĐ</p>
            ";

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}