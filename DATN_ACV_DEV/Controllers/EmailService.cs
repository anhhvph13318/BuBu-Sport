using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using System.Threading.Tasks;
using System.Text;
using System.Net.WebSockets;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.Order_DTO;

namespace DATN_ACV_DEV.Controllers
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type, List<OrderProduct>? products, IList<OrderItem>? orderItems);
    }

    public class EmailService : IEmailService
    {
        
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly DBContext _context;
        public EmailService(IConfiguration configuration, DBContext context )
        {
            _context = context;
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _smtpUsername = "sprtbubu@gmail.com";
            _smtpPassword = "luys adyn vknr bdtp";
            _fromEmail = "sprtbubu@gmail.com";
            _fromName = "BuBuSport";
        }

        public async Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type, List<OrderProduct>? products, IList<OrderItem>? orderItems)
        {
            string productTable = string.Empty;
            string productTablee = string.Empty;
            if (products != null)
            {
                foreach (var item in products)
                {
                    if (item.sizeName == null && item.colorName == null)
                    {
                        item.sizeName = _context.TbSizes.Where(c => c.Id == _context.TbProductDetails.Where(a => a.Id == item.productId).Select(a => a.SizeId).FirstOrDefault()).Select(c => c.SizeName).FirstOrDefault();
                        item.colorName = _context.TbColors.Where(c => c.Id == _context.TbProductDetails.Where(a => a.Id == item.productId).Select(a => a.ColorId).FirstOrDefault()).Select(c => c.Name).FirstOrDefault();
                    }
                }
                decimal total = 0;
                int stt = 1;

                string tableRows = "";
                foreach (var item in products)
                {
                    decimal itemTotal = (item.price ?? 0) * item.quantity;
                    total += itemTotal;

                    tableRows += $@"
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{stt++}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>
                            <img src='{item.url}' alt='Ảnh' style='width: 50px; height: 50px; object-fit: cover;'/>
                        </td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{item.productCode}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{item.productName}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{item.quantity}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{item.colorName}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{item.sizeName}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{(item.price ?? 0):n0}đ</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{itemTotal:n0}đ</td>
                </tr>";
                }
                productTable = $@"
                <h4>Chi tiết đơn hàng:</h4>
                <table style='border-collapse: collapse; width: 100%;'>
                    <thead>
                        <tr style='background-color: #f2f2f2;'>
                            <th style='border: 1px solid #ddd; padding: 8px;'>STT</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Ảnh</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Mã sản phẩm</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Tên sản phẩm</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Màu sắc</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Size</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan='8' style='border: 1px solid #ddd; padding: 8px; text-align: right;'><strong>Tổng tiền:</strong></td>
                            <td style='border: 1px solid #ddd; padding: 8px;'><strong>{total:n0}đ</strong></td>
                        </tr>
                    </tfoot>
                </table><br/>";
            }
            if (orderItems != null)
            {
                decimal total = 0;
                int stt = 1;

                string tableRows = "";
                foreach (var item in orderItems)
                {
                    decimal itemTotal = item.Price * item.Quantity;
                    total += itemTotal;

                    tableRows += $@"
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{stt++}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>
                            <img src='{item.ProductImage}' alt='Ảnh' style='width: 50px; height: 50px; object-fit: cover;'/>
                        </td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Code}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.ProductName}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Size}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Color}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Quantity}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Price:n0}đ</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{itemTotal:n0}đ</td>
                    </tr>";
                }

                productTablee = $@"
                <h4>Chi tiết đơn hàng:</h4>
                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;'>
                    <thead>
                        <tr style='background-color: #f2f2f2;'>
                            <th style='border: 1px solid #ddd; padding: 8px;'>STT</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Ảnh</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Mã sản phẩm</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Tên sản phẩm</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Size</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Màu</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                            <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>
                    {tableRows}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan='8' style='border: 1px solid #ddd; padding: 8px; text-align: right;'><strong>Tổng tiền:</strong></td>
                            <td style='border: 1px solid #ddd; padding: 8px;'><strong>{total:n0}đ</strong></td>
                        </tr>
                    </tfoot>
                </table><br/>";
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email không hợp lệ, bỏ qua gửi email xác nhận.");
                return;
            }
            string contentProduct = productTable != string.Empty ? productTable : productTablee;

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
                {contentProduct}
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
                <p>Đây là mã để xác thực hành động hủy đơn hàng của bạn : {status}</p>
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