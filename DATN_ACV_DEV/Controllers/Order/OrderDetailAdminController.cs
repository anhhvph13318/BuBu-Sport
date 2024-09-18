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
            .Include(e => e.Voucher)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                Code = e.OrderCode,
                IsDraft = e.IsDraft,
                PaymentMethod = e.PaymentMethod,
                Customer = new CustomerInfo
                {
                    Id = e.Customer.Id,
                    Name = e.Customer.Name,
                    Address = e.Customer.Adress,
                    PhoneNumber = e.Customer.Phone
                },
                ShippingInfo = e.AddressDelivery == null ? null : new ShippingInfo
                {
                    Name = e.AddressDelivery!.ReceiverName,
                    PhoneNumber = e.AddressDelivery!.ReceiverPhone,
                    Address = e.AddressDelivery.ProvinceName
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalDiscount = e.TotalAmountDiscount.Value,
                    ShippingFee = e.AmountShip ?? 0,
                    TotalTax = e.TotalAmount == 0 ? 0 : e.TotalAmount * 10 / 100,
                    TotalAmount = e.TotalAmount,
                    Status = e.Status ?? 0,
                    VoucherId = e.VoucherId,
                    VoucherCode = e.Voucher.Code ?? string.Empty
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
                IsCustomerTakeYourSelf = e.IsCustomerTakeYourself,
                IsSameAsCustomerAddress = e.IsShippingAddressSameAsCustomerAddress,
                PaymentMethodName = e.PaymentMethod == 2 ? "VNPay" : "Tiền mặt",
                StatusText = Common.ConvertStatusOrder(e.Status ?? 0),
                Status = e.Status ?? 0,
                Items = e.TbOrderDetails.Select(d => new OrderItem()
                {
                    Id = d.ProductId,
                    Price = d.Product.Price,
                    Quantity = d.Quantity,
                    ProductImage = d.Product.Image.Url,
                    ProductName = d.Product.Name
                }),
            }).FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

        if(order != null && order.ShippingInfo == null)
            order.ShippingInfo = ShippingInfo.Default();

        return Ok(new BaseResponse<OrderDetail>()
        {
            Data = order ?? new OrderDetail()
        });
    }

    [HttpGet]
    [Route("/api/admin/orders/search/{s}")]
    public async Task<IActionResult> Search([FromRoute] string s)
    {
        var isGuid = Guid.TryParse(s, out var id);

        var order = await _context.TbOrders.AsNoTracking()
            .Include(e => e.Customer)
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .ThenInclude(e => e.Image)
            .Include(e => e.AddressDelivery)
            .Include(e => e.Voucher)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                Code = e.OrderCode,
                IsDraft = e.IsDraft,
                Customer = new CustomerInfo
                {
                    Id = e.Customer.Id,
                    Name = e.Description ?? e.Customer.Name,
                    Address = e.Customer.Adress,
                    PhoneNumber = e.Customer.Phone
                },
                ShippingInfo = e.AddressDelivery == null ? new ShippingInfo
                {
                    Name = "",
                    PhoneNumber = "",
                    Address = ""
                } : new ShippingInfo
                {
                    Name = e.AddressDelivery!.ReceiverName,
                    PhoneNumber = e.AddressDelivery!.ReceiverPhone,
                    Address = $"{e.AddressDelivery.ProvinceName}"
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalDiscount = e.TotalAmountDiscount.Value,
                    ShippingFee = e.AmountShip ?? 0,
                    TotalTax = e.TotalAmount == 0 ? 0 : e.TotalAmount * 10 / 100,
                    TotalAmount = e.TotalAmount,
                    Status = e.Status ?? 0,
                    VoucherId = e.VoucherId,
                    VoucherCode = e.Voucher.Code ?? string.Empty
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
                IsCustomerTakeYourSelf = e.IsCustomerTakeYourself,
                IsSameAsCustomerAddress = e.IsShippingAddressSameAsCustomerAddress,
                PaymentMethodName = e.PaymentMethod == 1 ? "Thanh toán khi nhận hàng" : "Thanh toán qua công VNPay",
                StatusText = Common.ConvertStatusOrder(e.Status ?? 0),
                Status = e.Status ?? 0,
                Items = e.TbOrderDetails.Select(d => new OrderItem()
                {
                    Id = d.ProductId,
                    Price = d.Product.Price,
                    Quantity = d.Quantity,
                    ProductImage = d.Product.Image.Url,
                    ProductName = d.Product.Name
                }),
                Created = e.CreateDate
            }).Where(e => (isGuid && e.Id == id) || (!isGuid && e.Customer.PhoneNumber == s)).OrderBy(c => c.Status).ThenByDescending(c => c.Created).ToListAsync();


        return Ok(new BaseResponse<List<OrderDetail>>()
        {
            Data = order ?? new List<OrderDetail>()
        });
    }

    [HttpGet]
    [Route("/api/storefront/orders/search/{s}")]
    public async Task<IActionResult> SearchStorefront([FromRoute] string s)
    {
        var isGuid = Guid.TryParse(s, out var id);

        var order = await _context.TbOrders.AsNoTracking()
            .Include(e => e.Customer)
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .ThenInclude(e => e.Image)
            .Include(e => e.AddressDelivery)
            .Include(e => e.Voucher)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                Code = e.OrderCode,
                IsDraft = e.IsDraft,
                Customer = new CustomerInfo
                {
                    Id = e.Customer.Id,
                    Name = e.Customer.Name,
                    Address = e.Customer.Adress,
                    PhoneNumber = e.Customer.Phone
                },
                ShippingInfo = e.AddressDelivery == null ? new ShippingInfo
                {
                    Name = "",
                    PhoneNumber = "",
                    Address = ""
                } : new ShippingInfo
                {
                    Name = e.AddressDelivery!.ReceiverName,
                    PhoneNumber = e.AddressDelivery!.ReceiverPhone,
                    Address = $"{e.AddressDelivery.ProvinceName}"
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalDiscount = e.TotalAmountDiscount.Value,
                    ShippingFee = e.IsCustomerTakeYourself ? 0 : 30000,
                    TotalTax = e.TotalAmount == 0 ? 0 : e.TotalAmount * 10 / 100,
                    TotalAmount = e.TotalAmount,
                    Status = e.Status ?? 0,
                    VoucherId = e.VoucherId,
                    VoucherCode = e.Voucher.Code ?? string.Empty
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
                IsCustomerTakeYourSelf = e.IsCustomerTakeYourself,
                IsSameAsCustomerAddress = e.IsShippingAddressSameAsCustomerAddress,
                PaymentMethodName = e.PaymentMethod == 1 ? "Thanh toán khi nhận hàng" : "Thanh toán qua công VNPay",
                StatusText = Common.ConvertStatusOrder(e.Status ?? 0),
                Status = e.Status ?? 0,
                Items = e.TbOrderDetails.Select(d => new OrderItem()
                {
                    Id = d.ProductId,
                    Price = d.Product.Price,
                    Quantity = d.Quantity,
                    ProductImage = d.Product.Image.Url,
                    ProductName = d.Product.Name
                }),
            }).Where(e => (isGuid && e.Id == id) || (!isGuid && e.Customer.PhoneNumber == s) || (e.Code == s)).ToListAsync();


        return Ok(new BaseResponse<List<OrderDetail>>()
        {
            Data = order ?? new List<OrderDetail>()
        });
    }
}