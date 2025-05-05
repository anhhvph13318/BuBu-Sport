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

        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Nếu không có ngày được chọn, mặc định là 7 ngày gần nhất
                if (!startDate.HasValue)
                {
                    startDate = DateTime.Today.AddDays(-6); // 7 ngày gần nhất (bao gồm hôm nay)
                }

                if (!endDate.HasValue)
                {
                    endDate = DateTime.Today;
                }

                // Đảm bảo ngày bắt đầu không lớn hơn ngày kết thúc
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }

                // Lưu giá trị ngày vào ViewBag để sử dụng trong View
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

                // Tính số ngày giữa hai mốc thời gian
                int days = (int)(endDate.Value - startDate.Value).TotalDays + 1;

                // Gọi API với tham số startDate và endDate
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"http://localhost:5059/api/revenue/stats?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var revenueData = JsonSerializer.Deserialize<RevenueData>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

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
        public decimal TotalRevenuePeriod { get; set; }
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