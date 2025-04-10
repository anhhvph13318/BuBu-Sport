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
    public class ProductNames
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
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
                amountDiscount = e.TotalAmountDiscount,
                amountShip = e.AmountShip,
                id = e.Id,
                nameCustomer = _context.TbCustomers.Where(c=>c.Id == e.CustomerId).Select(c=>c.Name).FirstOrDefault() ,
                status = Common.ConvertStatusOrder(e.Status ?? 0),
                // Lấy tên sản phẩm từ ProductDetail → Product
                products = "",

            }).ToListAsync();
            List<ProductNames> productNames = new List<ProductNames>();
            foreach (var item in orders)
            {
                var productIds = _context.TbOrderDetails
                    .Where(c => c.OrderId == item.id)
                    .Select(c => c.ProductId)
                    .ToList();

                foreach (var productDetailId in productIds)
                {
                    var productName = (from pd in _context.TbProductDetails
                                       join p in _context.TbProducts on pd.ProductId equals p.Id
                                       where pd.Id == productDetailId
                                       select p.Name).FirstOrDefault();

                    if (!string.IsNullOrEmpty(productName))
                    {
                        productNames.Add(new ProductNames
                        {
                            OrderId = item.id,
                            ProductName = productName
                        });
                    }
                }
            }
        // Nhóm danh sách sản phẩm theo OrderId
        var productNamesByOrder = productNames
            .GroupBy(p => p.OrderId)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(p => p.ProductName)));

        // Gán lại giá trị products cho từng đơn hàng
        foreach (var order in orders)
        {
            if (productNamesByOrder.TryGetValue(order.id, out var productList))
            {
                order.products = productList;
            }
        }
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
            .Include(e => e.Voucher)
            .Where(e => e.Status != 7)
            .OrderByDescending(e => e.CreateDate)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                Code = e.OrderCode!,
                IsCustomerTakeYourSelf = e.IsCustomerTakeYourself,
                IsSameAsCustomerAddress = e.IsShippingAddressSameAsCustomerAddress,
                Status = e.Status.Value,
                StatusText = Utility.Common.ConvertStatusOrder(e.Status.Value),
                IsDraft = e.IsDraft,
                OrderTypeName = GetOrderTypeName(e.OrderCode!),
                Customer = new CustomerInfo
                {
                    Id = e.Id,
                    Name = _context.TbCustomers.Where(c=>c.Id == e.CustomerId).Select(c=>c.Name).FirstOrDefault(),
                    Address = e.Customer.Adress,
                    PhoneNumber = e.Customer.Phone,
                    Email = _context.TbAccounts.Where(c=>c.CustomerId == e.CustomerId).Select(c=>c.Email).FirstOrDefault()
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalDiscount = e.TotalAmountDiscount!.Value,
                    TotalAmount = e.TotalAmount,
                    VoucherId = e.VoucherId,
                    ShippingFee = e.AmountShip ?? 0
                },
                Items = e.TbOrderDetails.Select(e => new OrderItem()
                {
                    Id = e.Id,
                    ProductImage = e.Product.tb_Image.Url,
                    Price = e.Product.Price,
                    ProductName = e.Product.Name,
                    Quantity = e.Quantity
                }),
                ShippingInfo = new ShippingInfo()
                {
                    Address = e.AddressDelivery.ProvinceName,
                    PhoneNumber = e.AddressDelivery.ReceiverPhone,
                    Name = e.AddressDelivery.ReceiverName
                },
                Voucher = e.VoucherId == null 
                    ? new Model_DTO.Voucher_DTO.VoucherDTO()
                    : new Model_DTO.Voucher_DTO.VoucherDTO
                    {
                        Id = e.Voucher.Id,
                        Code = e.Voucher.Code,
                        MaxDiscount = e.Voucher.MaxDiscount,
                        Discount = e.Voucher.Discount,
                        Unit = e.Voucher.Unit,
                        Type = e.Voucher.Type,
                    },
                Created = e.CreateDate //VANH
            }).ToListAsync();

        return Ok(new BaseResponse<IEnumerable<OrderDetail>>()
        {
            Data = orders
        });
    }

    private static string GetOrderTypeName(string code)
    {
        if (code.StartsWith("OFF"))
            return "Tại quầy";
        else if (code.StartsWith("TEMP"))
            return "Nháp";
        else
            return "Online";
    }
}