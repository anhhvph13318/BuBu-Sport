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
        Guid orderId;
        if (!Guid.TryParse(id, out orderId))
            return BadRequest("Id không hợp lệ.");

        var orderEntity = await _context.TbOrders
            .Include(o => o.TbOrderDetails)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(e => e.Id == orderId);

        if (orderEntity == null)
            return NotFound("Không tìm thấy đơn hàng.");
        var customerId = _context.TbOrders.Where(c => c.Id == Guid.Parse(id)).Select(c => c.CustomerId).FirstOrDefault();
        var customer = _context.TbCustomers.Where(c => c.Id == customerId).FirstOrDefault();

        var orderDetails = _context.TbOrderDetails.Where(c => c.OrderId == Guid.Parse(id)).ToList();

        //var product = orderDetails?.Product;
        //var image = _context.TbImages.Where(c => c.Id == product.ImageId).Select(c => c.Url).FirstOrDefault();

        var order = new OrderDetail()
        {
            Id = orderEntity.Id,
            Code = orderEntity.OrderCode,
            IsDraft = orderEntity.IsDraft,
            PaymentMethod = orderEntity.PaymentMethod,
            Customer = customer == null ? null : new CustomerInfo
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Adress,
                PhoneNumber = customer.Phone,
                Email = _context.TbAccounts.Where(c=>c.CustomerId == customer.Id).Select(c=>c.Email).FirstOrDefault() != null ?
				_context.TbAccounts.Where(c => c.CustomerId == customer.Id).Select(c => c.Email).FirstOrDefault():
                _context.TbAddressDeliveries.Where(c=>c.Id == customer.TbOrders.Select(a=>a.AddressDeliveryId).FirstOrDefault()).Select(c=>c.receiverEmail).FirstOrDefault()
			},
            ShippingInfo = customer == null ? null : new ShippingInfo
            {
                Name = customer.Name,
                PhoneNumber = customer.Phone,
                Address = customer.Adress
            },
            PaymentInfo = new PaymentInfo
            {
                TotalDiscount = orderEntity.TotalAmountDiscount ?? 0,
                ShippingFee = orderEntity.AmountShip ?? 0,
                TotalTax = orderEntity.TotalAmount == 0 ? 0 : orderEntity.TotalAmount * 10 / 100,
                TotalAmount = orderEntity.TotalAmount,
                Status = orderEntity.Status ?? 0,
                VoucherId = orderEntity.VoucherId,
                VoucherCode = orderEntity.Voucher?.Code ?? string.Empty
            },
            Voucher = orderEntity.Voucher == null ? null : new Model_DTO.Voucher_DTO.VoucherDTO
            {
                Id = orderEntity.Voucher.Id,
                Code = orderEntity.Voucher.Code,
                MaxDiscount = orderEntity.Voucher.MaxDiscount,
                Discount = orderEntity.Voucher.Discount,
                Unit = orderEntity.Voucher.Unit,
                Type = orderEntity.Voucher.Type,
            },
            IsCustomerTakeYourSelf = orderEntity.IsCustomerTakeYourself,
            IsSameAsCustomerAddress = orderEntity.IsShippingAddressSameAsCustomerAddress,
            PaymentMethodName = orderEntity.PaymentMethod == 2 ? "VNPay" : "Tiền mặt",
            StatusText = Common.ConvertStatusOrder(orderEntity.Status ?? 0),
            Status = orderEntity.Status ?? 0,

            Items = orderDetails
            .Select(orderDetail => new OrderItem
            {
                Id = orderDetail.ProductId,
                Code = _context.TbProducts.Where(c => c.Id ==
                _context.TbProductDetails.Where(a => a.Id == orderDetail.ProductId).Select(a => a.ProductId).FirstOrDefault()).Select(c => c.Code).FirstOrDefault(),
                Price = _context.TbProductDetails.Where(c => c.Id == orderDetail.ProductId).Select(c => c.Price).FirstOrDefault(),
                Quantity = orderDetail.Quantity,
                ProductImage = _context.TbImages.Where(c => c.Id ==
                _context.TbProductDetails.Where(a => a.Id == orderDetail.ProductId).Select(a => a.ImageId).FirstOrDefault()).Select(c => c.Url).FirstOrDefault(),
                Size = _context.TbSizes.Where(c => c.Id ==
                _context.TbProductDetails.Where(a => a.Id == orderDetail.ProductId).Select(a => a.SizeId).FirstOrDefault()).Select(c => c.SizeName).FirstOrDefault(),
                Color = _context.TbColors.Where(c=>c.Id == 
                _context.TbProductDetails.Where(a=>a.Id == orderDetail.ProductId).Select(a=>a.ColorId).FirstOrDefault()).Select(c=>c.Name).FirstOrDefault(),
                ProductName = _context.TbProducts.Where(c => c.Id ==
                _context.TbProductDetails.Where(a => a.Id == orderDetail.ProductId).Select(a => a.ProductId).FirstOrDefault()).Select(c => c.Name).FirstOrDefault()
            })
            .ToList(),
            Created = orderEntity.CreateDate,
        };
        //foreach (var item in orderDetails.Order)
        //{
        //    new OrderItem()
        //    {
        //        Id = item.Id ?? Guid.Empty,
        //        Price = item?.Price ?? 0,
        //        Quantity = item.Quantity,
        //        //ProductImage = orderDetails.ProductId,
        //        //Size = product.Size,
        //        //Color = product.Color
        //        ProductName = item?.Name ?? "Không xác định"
        //    }
        //}
        return Ok(new BaseResponse<OrderDetail>
        {
            Data = order
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
            .ThenInclude(e => e.ImageId)
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
                    ProductImage = d.Product.tb_Image.Url,
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
        try
        {
            var isGuid = Guid.TryParse(s, out var id);
            var order = await _context.TbOrders.AsNoTracking()
                .Include(e => e.Customer)
                .Include(e => e.TbOrderDetails)
                .ThenInclude(e => e.Product)
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
                        PhoneNumber = e.AddressDelivery.ReceiverPhone,
                        Address = $"{e.AddressDelivery.ProvinceName}"
                    },
                    PaymentInfo = new PaymentInfo
                    {
                        TotalDiscount = e.TotalAmountDiscount ?? 0,
                        ShippingFee = e.IsCustomerTakeYourself ? 0 : 0,
                        TotalTax = e.TotalAmount == 0 ? 0 : e.TotalAmount * 10 / 100,
                        TotalAmount = e.TotalAmount,
                        Status = e.Status ?? 0,
                        VoucherId = e.VoucherId,
                        VoucherCode = e.Voucher != null ? e.Voucher.Code : "",
                        PaymentStatus = (e.PaymentMethod == 2 && e.PaymentStatus == 1) || e.Status == Utility.Utility.ORDER_STATUS_DONE
                            ? "Đã thanh toán"
                            : "Chưa thanh toán"
                    },
                    Voucher = e.Voucher == null
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
                        Id = _context.TbOrderDetails.Where(c => c.Id == d.Id).Select(c => c.ProductId).FirstOrDefault(),
                        Price = _context.TbProductDetails.Where(c => c.Id == _context.TbOrderDetails.Where(c => c.Id == d.Id).Select(c => c.ProductId).FirstOrDefault()).Select(c => c.Price).FirstOrDefault(),
                        Quantity = d.Quantity,
                        ProductImage = _context.TbImages.Where(c => c.Id == _context.TbProductDetails.Where(a => a.Id == d.ProductId).Select(c => c.ImageId).FirstOrDefault()).Select(c => c.Url).FirstOrDefault(),
                        ProductName = _context.TbProducts.Where(v=>v.Id == _context.TbProductDetails.Where(c => c.Id == 
                        _context.TbOrderDetails.Where(c => c.Id == d.Id).Select(c => c.ProductId).FirstOrDefault()).Select(c => c.ProductId).FirstOrDefault()).Select(v=>v.Name).FirstOrDefault(),
                        Code = _context.TbProducts.Where(v => v.Id == _context.TbProductDetails.Where(c => c.Id ==
                        _context.TbOrderDetails.Where(c => c.Id == d.Id).Select(c => c.ProductId).FirstOrDefault()).Select(c => c.ProductId).FirstOrDefault()).Select(v => v.Code).FirstOrDefault(),
                        Color = _context.TbColors.Where(c => c.Id == _context.TbProductDetails.Where(a => a.Id == d.ProductId).Select(c => c.ColorId).FirstOrDefault()).Select(c => c.Name).FirstOrDefault(),
                        Size = _context.TbSizes.Where(c => c.Id == _context.TbProductDetails.Where(a => a.Id == d.ProductId).Select(c => c.SizeId).FirstOrDefault()).Select(c => c.SizeName).FirstOrDefault(),
                    }),
                    Created = e.CreateDate
                })
                .Where(e => !e.IsDraft && (isGuid && e.Id == id) || (!isGuid && e.Customer.PhoneNumber == s) || (e.Code == s))
                .ToListAsync();

            return Ok(new BaseResponse<List<OrderDetail>>()
            {
                Data = order ?? new List<OrderDetail>()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi truy vấn đơn hàng", error = ex.Message });
        }
    }
}