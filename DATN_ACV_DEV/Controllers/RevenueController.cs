using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DATN_ACV_DEV.Entity;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly DBContext _context;

        public RevenueController(DBContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetRevenueStats(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Ngày hiện tại (mặc định là hôm nay)
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);

                // Xử lý tham số ngày
                if (!startDate.HasValue)
                {
                    startDate = today.AddDays(-6); // Mặc định 7 ngày gần nhất (bao gồm hôm nay)
                }

                if (!endDate.HasValue)
                {
                    endDate = today;
                }

                // Đảm bảo ngày bắt đầu không lớn hơn ngày kết thúc
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }

                // Tuần này
                var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                var endOfThisWeek = startOfThisWeek.AddDays(6);
                var startOfLastWeek = startOfThisWeek.AddDays(-7);
                var endOfLastWeek = startOfLastWeek.AddDays(6);

                // Tháng này
                var startOfThisMonth = new DateTime(today.Year, today.Month, 1);
                var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
                var startOfLastMonth = startOfThisMonth.AddMonths(-1);
                var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1);

                // Truy vấn doanh thu (chỉ tính đơn hàng hoàn thành)
                var query = _context.TbOrders
                    .Where(o => o.Status == 7); // Giả định Status = 7 là hoàn thành

                // Lấy tất cả đơn hàng không phải bản nháp
                var allOrders = await _context.TbOrders.ToListAsync();

                // Thống kê theo từng trạng thái
                var totalOrders = allOrders.Count;
                var waitingOrders = allOrders.Count(o => o.Status == 0);         // Chờ xác nhận
                var confirmedOrders = allOrders.Count(o => o.Status == 1);       // Đã xác nhận
                var preparingOrders = allOrders.Count(o => o.Status == 4);      // Đang chuẩn bị hàng
                var deliveringOrders = allOrders.Count(o => o.Status == 2);      // Đang giao hàng
                var deliverySuccessOrders = allOrders.Count(o => o.Status == 9);      // Giao hàng thành công
                var completedOrders = allOrders.Count(o => o.Status == 7);       // Hoàn thành
                var cancelledOrders = allOrders.Count(o => o.Status == 3);       // Đã hủy

                // Doanh thu hôm nay
                var revenueToday = await query
                    .Where(o => o.CreateDate.Date == today)
                    .SumAsync(o => o.TotalAmount);

                // Doanh thu hôm qua
                var revenueYesterday = await query
                    .Where(o => o.CreateDate.Date == yesterday)
                    .SumAsync(o => o.TotalAmount);

                // Doanh thu tuần này
                var revenueThisWeek = await query
                    .Where(o => o.CreateDate.Date >= startOfThisWeek && o.CreateDate.Date <= endOfThisWeek)
                    .SumAsync(o => o.TotalAmount);

                var revenueLastWeek = await query
                    .Where(o => o.CreateDate.Date >= startOfLastWeek && o.CreateDate.Date <= endOfLastWeek)
                    .SumAsync(o => o.TotalAmount);

                // Doanh thu tháng này
                var revenueThisMonth = await query
                    .Where(o => o.CreateDate.Date >= startOfThisMonth && o.CreateDate.Date <= endOfThisMonth)
                    .SumAsync(o => o.TotalAmount);

                var revenueLastMonth = await query
                    .Where(o => o.CreateDate.Date >= startOfLastMonth && o.CreateDate.Date <= endOfLastMonth)
                    .SumAsync(o => o.TotalAmount);

                // Tính số ngày trong khoảng thời gian đã chọn
                int days = (int)(endDate.Value - startDate.Value).TotalDays + 1;

                // Doanh thu theo khoảng thời gian tùy chỉnh
                var dailyRevenues = new decimal[days];
                var dates = new string[days];

                for (int i = 0; i < days; i++)
                {
                    var date = startDate.Value.AddDays(i);
                    dates[i] = date.ToString("dd/MM");
                    dailyRevenues[i] = await query
                        .Where(o => o.CreateDate.Date == date.Date)
                        .SumAsync(o => o.TotalAmount);
                }

                // Tổng doanh thu theo khoảng thời gian
                var totalRevenuePeriod = dailyRevenues.Sum();

                // Tính tỷ lệ tăng trưởng (%)
                var growthToday = revenueYesterday != 0 ? (revenueToday - revenueYesterday) / revenueYesterday * 100 : 0;
                var growthThisWeek = revenueLastWeek != 0 ? (revenueThisWeek - revenueLastWeek) / revenueLastWeek * 100 : 0;
                var growthThisMonth = revenueLastMonth != 0 ? (revenueThisMonth - revenueLastMonth) / revenueLastMonth * 100 : 0;

                // Trả về kết quả
                var result = new
                {
                    RevenueToday = revenueToday,
                    GrowthToday = Math.Round(growthToday, 2),
                    CompareDate = yesterday.ToString("dd-MM-yyyy"),
                    RevenueThisWeek = revenueThisWeek,
                    GrowthThisWeek = Math.Round(growthThisWeek, 2),
                    RevenueThisMonth = revenueThisMonth,
                    GrowthThisMonth = Math.Round(growthThisMonth, 2),
                    DailyRevenues = dailyRevenues,
                    Dates = dates,
                    TotalRevenuePeriod = totalRevenuePeriod,
                    TotalOrders = totalOrders,
                    ConfirmedOrders = confirmedOrders,
                    WaitingOrders = waitingOrders,
                    PreparingOrders = preparingOrders,
                    DeliveringOrders = deliveringOrders,
                    DeliverySuccessOrders = deliverySuccessOrders,
                    CompletedOrders = completedOrders,
                    CancelledOrders = cancelledOrders
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tính toán thống kê doanh thu.", error = ex.Message });
            }
        }
    }
}