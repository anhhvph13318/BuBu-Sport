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
        public async Task<IActionResult> GetRevenueStats(int days = 7) // Thêm tham số days, mặc định là 7
        {
            try
            {
                // Ngày hiện tại (01-04-2025, theo ngày thực tế)
                var today = DateTime.Today; // 01-04-2025
                var yesterday = today.AddDays(-1); // 31-03-2025, ngày hôm qua

                // Khoảng thời gian tùy chỉnh (mặc định 7 ngày qua)
                var startOfPeriod = today.AddDays(-days + 1); // Ví dụ: 26-03-2025 nếu days = 7

                // Tuần này (31-03-2025 đến 06-04-2025)
                var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // 31-03-2025
                var endOfThisWeek = startOfThisWeek.AddDays(6); // 06-04-2025
                var startOfLastWeek = startOfThisWeek.AddDays(-7); // 24-03-2025
                var endOfLastWeek = startOfLastWeek.AddDays(6); // 30-03-2025

                // Tháng này (04-2025)
                var startOfThisMonth = new DateTime(today.Year, today.Month, 1); // 01-04-2025
                var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1); // 30-04-2025
                var startOfLastMonth = startOfThisMonth.AddMonths(-1); // 01-03-2025
                var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1); // 31-03-2025

                // Truy vấn doanh thu (chỉ tính đơn hàng hoàn thành và không phải nháp)
                var query = _context.TbOrders
                    .Where(o => o.Status == 1 && !o.IsDraft); // Giả định Status = 1 là hoàn thành

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

                // Doanh thu theo khoảng thời gian tùy chỉnh
                var dailyRevenues = new decimal[days];
                var dates = new string[days];
                for (int i = 0; i < days; i++)
                {
                    var date = startOfPeriod.AddDays(i);
                    dates[i] = date.ToString("dd/MM");
                    dailyRevenues[i] = await query
                        .Where(o => o.CreateDate.Date == date)
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
                    TotalRevenuePeriod = totalRevenuePeriod
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