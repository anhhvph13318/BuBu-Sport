using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Order;

[ApiController]
public class OrderDetailAdminController : ControllerBase
{
    private readonly DBContext _context;

    public OrderDetailAdminController(DBContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("/api/admin/orders/{id}")]
    public async Task<IActionResult> Proccess([FromRoute] string id)
    {
        var order = await _context.TbOrders.AsNoTracking()
            .Include(e => e.Customer)
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .ThenInclude(e => e.Image)
            .Include(e => e.AddressDelivery)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                CustomerName = e.Customer.Name,
                PhoneNumber = e.AddressDelivery.ReceiverPhone,
                ShippingAddress = $"{e.AddressDelivery.WardName}, {e.AddressDelivery.DistrictName}, {e.AddressDelivery.ProvinceName}",
                PaymentMethodName = "Chuyển khoản ngân hàng",
                DiscountAmout = 0,
                VoucherDiscountAmount = 0,
                TotalAmount = e.TotalAmount ?? 0,
                Status = Common.ConvertStatusOrder(e.Status ?? 1),
                Items = e.TbOrderDetails.Select(d => new OrderItem()
                {
                    Id = d.Id,
                    Price = d.Product.Price,
                    Quantity = d.Quantity,
                    ProductImage = d.Product.Image.Url,
                    ProductName = d.Product.Name
                })
            }).FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

        return Ok(new BaseResponse<OrderDetail>()
        {
            Data = order ?? new OrderDetail()
        });
    }
}