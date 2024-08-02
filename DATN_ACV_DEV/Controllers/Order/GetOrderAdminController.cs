using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Order;

[ApiController]
public class GetOrderAdminController : ControllerBase
{
    private readonly DBContext _context;

    public GetOrderAdminController(DBContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("/api/admin/orders")]
    public async Task<IActionResult> Process(string? code = "", string? customerName = "", int status = 0)
    {
        var orders = await _context.TbOrders.AsNoTracking()
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .Include(e => e.Customer)
            .Where(e => (string.IsNullOrEmpty(code) || e.OrderCode == code)
                        && (string.IsNullOrEmpty(customerName) || e.Customer.Name.StartsWith(customerName))
                        && (status == 0 || e.Status == status))
            .OrderByDescending(e => e.CreateDate)
            .Select(e => new OrderListItem()
            {
                code = e.OrderCode,
                totalAmount = e.TotalAmount,
                id = e.Id,
                nameCustomer = e.Customer.Name,
                status = Common.ConvertStatusOrder(e.Status ?? 0),
                products = string.Join(", ", e.TbOrderDetails.Take(2).Select(e => e.Product.Name))
            }).ToListAsync();

        return Ok(new BaseResponse<IEnumerable<OrderListItem>>()
        {
            Data = orders
        });
    }

    [HttpGet]
    [Route("/api/admin/orders/not-completed")]
    public async Task<IActionResult> GetOrdersNotCompleted()
    {
        var orders = await _context.TbOrders.AsNoTracking()
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .Include(e => e.Customer)
            .Where(e => e.Status != 7)
            .OrderByDescending(e => e.CreateDate)
            .Select(e => new OrderListItem()
            {
                code = e.OrderCode,
                totalAmount = e.TotalAmount,
                id = e.Id,
                nameCustomer = e.Customer!.Name,
                status = Common.ConvertStatusOrder(e.Status ?? 0),
                products = string.Join(", ", e.TbOrderDetails.Take(2).Select(e => e.Product.Name))
            }).ToListAsync();

        return Ok(new BaseResponse<IEnumerable<OrderListItem>>()
        {
            Data = orders
        });
    }
}