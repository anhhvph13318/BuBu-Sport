using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GUI.Controllers
{
    public class RevenueController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RevenueController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int days = 7) // Thêm tham số days, mặc định là 7
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"http://localhost:5059/api/revenue/stats?days={days}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var revenueData = JsonSerializer.Deserialize<RevenueData>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    // Lưu giá trị days vào ViewBag để sử dụng trong View
                    ViewBag.SelectedDays = days;
                    return View(revenueData);
                }
                else
                {
                    TempData["Error"] = "Không thể lấy dữ liệu doanh thu từ API.";
                    return View(new RevenueData());
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Không thể kết nối đến API. Vui lòng kiểm tra xem DATN_ACV_DEV có đang chạy không.";
                return View(new RevenueData());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}";
                return View(new RevenueData());
            }
        }
    }

    public class RevenueData
    {
        public decimal RevenueToday { get; set; }
        public decimal GrowthToday { get; set; }
        public string CompareDate { get; set; }
        public decimal RevenueThisWeek { get; set; }
        public decimal GrowthThisWeek { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal GrowthThisMonth { get; set; }
        public decimal[] DailyRevenues { get; set; }
        public string[] Dates { get; set; }
        public decimal TotalRevenuePeriod { get; set; } // Đổi tên để phản ánh khoảng thời gian tùy chỉnh
        public int TotalOrders { get; set; }
        public int WaitingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int DeliveringOrders { get; set; }
        public int DeliverySuccessOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

    }
}