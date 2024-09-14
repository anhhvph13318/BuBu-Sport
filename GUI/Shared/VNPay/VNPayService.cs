using Azure;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using Microsoft.Extensions.Options;
using VNPAY_CS_ASPX;
using GUI.Shared.Common;
using GUI.Models.VNPay;

namespace GUI.Shared.VNPay
{
    public class VNPayService
    {
        private HttpService httpService;
        public CommonSettings _settings = new();
        public VNPayService(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;

        }
        public string SendRequest(string ipAddr, string orderId, decimal totalAmount, string bankCode = "")
        {
            //Get Config Info
            string vnp_Returnurl = _settings.vnp_Returnurl; //URL nhan ket qua tra ve 
            string vnp_Url = _settings.vnp_Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _settings.vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _settings.vnp_HashSecret; //Secret Key

            //Get payment input
            OrderInfo order = new OrderInfo();
            order.OrderId = orderId; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = totalAmount; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            order.CreatedDate = DateTime.Now;
            //Save order to db

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            //vnpay.AddRequestData("vnp_BankCode", bankCode);

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public string SendRequest(string ipAddr, string orderId, int total)
        {
            //Get Config Info
            string vnp_Returnurl = _settings.vnp_Returnurl; //URL nhan ket qua tra ve 
            string vnp_Url = _settings.vnp_Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _settings.vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _settings.vnp_HashSecret; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", total.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            //vnpay.AddRequestData("vnp_BankCode", bankCode);

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderId); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
    }
}
