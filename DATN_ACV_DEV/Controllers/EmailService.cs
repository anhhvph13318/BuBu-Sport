using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using System.Threading.Tasks;
using System.Text;
using System.Net.WebSockets;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using Newtonsoft.Json;

namespace DATN_ACV_DEV.Controllers
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type, List<OrderProduct>? products, IList<OrderItem>? orderItems, IList<OrderItem>? productsUOT);
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

        public async Task SendOrderConfirmationAsync(string? email, string? orderCode, string? customerName, string? phonenumber ,string? status,string? password,int? type, List<OrderProduct>? products, IList<OrderItem>? orderItems, IList<OrderItem>? productsUOT)
        {
            var outOfStockIds = productsUOT != null ? string.Join(",", productsUOT.Select(p => p.Id)) : null;
            string outOfStockTable = string.Empty;
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
            if(productsUOT != null)
            {
                // Tạo bảng cho sản phẩm hết hàng
                decimal total = 0;
                int stt = 1;

                string outOfStockTableRows = "";
                foreach (var item in productsUOT ?? new List<OrderItem>())
                {
                    decimal itemTotal = item.Price * item.Quantity;
                    total += itemTotal;
                    outOfStockTableRows += $@"
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

                // Bảng cho sản phẩm hết hàng
                outOfStockTable = $@"
                <h4>Sản phẩm hết hàng:</h4>
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
                        {outOfStockTableRows}
                    </tbody>
                <tfoot>
                                        <tr>
                                            <td colspan='8' style='border: 1px solid #ddd; padding: 8px; text-align: right;'><strong>Tổng tiền:</strong></td>
                                            <td style='border: 1px solid #ddd; padding: 8px;'><strong>{total:n0}đ</strong></td>
                                        </tr>
                                    </tfoot>
                </table>";
            }    
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email không hợp lệ, bỏ qua gửi email xác nhận.");
                return;
            }

            string contentProduct = productTable != string.Empty ? productTable : productTablee;
            var orderId = _context.TbOrders.Where(c=>c.OrderCode == orderCode).Select(c=>c.Id).FirstOrDefault();
            var baseUrl = "http://localhost:5011";
            var linkConfirmPartial = $"{baseUrl}/orders/confirm-partial?orderId={orderId}&outOfStock={outOfStockIds}";
            var linkCancelOrder = $"{baseUrl}/orders/confirm-cancel?orderId={orderId}";
            var linkWaitForStock = $"{baseUrl}/orders/wait-stock?orderId={orderId}";
            string buttonPartial = "";
            if (productsUOT != null && orderItems != null && orderItems.Count() != productsUOT.Count())
            {
                buttonPartial = $@"
                <a href='{linkConfirmPartial}' style='
                    display: inline-block;
                    background-color: #28a745;
                    color: white;
                    padding: 10px 20px;
                    margin-right: 10px;
                    border-radius: 5px;
                    text-decoration: none;
                    font-weight: bold;
                '>Nhận phần còn lại</a>";
            }
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
  
            var lastproduct = $@"
              <h2>Trạng thái đơn hàng BuBu Sport</h2>
              <p>Xin chào {customerName},</p>
              <p><strong>Số điện thoại:</strong> {phonenumber}</p>
              <p><strong>Mã hóa đơn:</strong> {orderCode}</p>
              <p>Chúng tôi xin thông báo rằng trong đơn hàng của bạn có một hoặc vài sản phẩm đã hết hàng tạm thời.</p>
              <p>Danh sách các sản phẩm trong giỏ hàng của bạn:</p>
              {productTablee}

              <p>Danh sách các sản phẩm hết hàng:</p>
              {outOfStockTable}

              <p>
                Bạn có thể lựa chọn cách xử lý đơn hàng bên dưới:
              </p>
                <div style=""margin: 20px 0;"">
                {buttonPartial}

                <a href='{linkCancelOrder}' style='
                  display: inline-block;
                  background-color: #dc3545;
                  color: white;
                  padding: 10px 20px;
                  margin-right: 10px;
                  border-radius: 5px;
                  text-decoration: none;
                  font-weight: bold;
                '>Hủy đơn hàng</a>

                <a href='{linkWaitForStock}' style='
                  display: inline-block;
                  background-color: #ffc107;
                  color: black;
                  padding: 10px 20px;
                  border-radius: 5px;
                  text-decoration: none;
                  font-weight: bold;
                '>Chờ đủ hàng</a>
              </div>
              <p>Nếu bạn không thực hiện yêu cầu nào trong số trên, đơn hàng sẽ được xử lý mặc định sau 24 giờ.</p>
              <p>Cảm ơn bạn đã tin tưởng sử dụng dịch vụ của chúng tôi!</p>
              <p>Trân trọng,</p>
              <p><strong>Nhóm chăm sóc khách hàng BuBu Sport</strong></p>
            ";
            var builder = new BodyBuilder();
            builder.HtmlBody = type == 0
                ? resetpass
                : type == 2
                    ? cancelorder
                    : type == 3
                        ? lastproduct
                        : orderstatus;


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