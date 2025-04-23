using DATN_ACV_DEV.Entity;
using GUI.Controllers.Shared;
using GUI.Models.DTOs;
using GUI.Models.DTOs.Discount_DTO;
using Microsoft.AspNetCore.Mvc;

namespace GUI.Controllers
{
    [Controller]
    [Route("Discount")]
    public class DiscountController : ControllerSharedBase
    {
        private readonly DBContext _context;
        private readonly UserSession _session;

        public DiscountController(DBContext context, UserSession session)
        {
            _context = context;
            _session = session;
        }
        [HttpGet]
        public IActionResult Index(string name, string discountType, int? status)
        {
            var query = _context.TbDiscounts.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.Name.Contains(name));

            if (!string.IsNullOrEmpty(discountType))
                query = query.Where(d => d.DiscountType == discountType);

            if (status.HasValue)
            {
                var now = DateTime.Now;
                switch (status.Value)
                {
                    case 1: // Chưa bắt đầu
                        query = query.Where(d => d.StartDate > now);
                        break;
                    case 2: // Hoạt động
                        query = query.Where(d => d.StartDate <= now && d.EndDate >= now);
                        break;
                    case 3: // Hết hiệu lực
                        query = query.Where(d => d.EndDate < now);
                        break;
                }
            }

            var model = new DiscountListViewModel
            {
                Discounts = query.ToList()
            };

            return View(model);
        }
        [HttpPost]
        [Route("CreateDiscount")]
        public IActionResult CreateOrUpdateDiscount(CreateDiscountViewModel model, string actionType)
        {
            if (actionType == "update")
            {
                var tbDiscount = _context.TbDiscounts.FirstOrDefault(c => c.Id == model.Id);

                if (tbDiscount == null)
                {
                    return NotFound("Không tìm thấy khuyến mại cần cập nhật.");
                }

                tbDiscount.Name = model.Name;
                tbDiscount.DiscountType = model.DiscountType;
                tbDiscount.DiscountValue = model.DiscountValue;
                tbDiscount.MaxDiscountAmount = model.DiscountType == "percent" ? model.MaxDiscountAmount : null;
                tbDiscount.StartDate = model.StartDate;
                tbDiscount.EndDate = model.EndDate;

                // Cập nhật lại các sản phẩm nếu cần:
                var oldProducts = _context.TbDiscountProducts.Where(p => p.DiscountId == tbDiscount.Id);
                _context.TbDiscountProducts.RemoveRange(oldProducts);

                foreach (var productId in model.ProductIds)
                {
                    _context.TbDiscountProducts.Add(new TbDiscountProduct
                    {
                        Id = Guid.NewGuid(),
                        DiscountId = tbDiscount.Id,
                        ProductId = productId
                    });
                }

                _context.SaveChanges();
            }
            else {
                // Tạo đối tượng Discount mới
                var discount = new TbDiscount
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    DiscountType = model.DiscountType,
                    DiscountValue = model.DiscountValue,
                    MaxDiscountAmount = model.DiscountType == "percent" ? model.MaxDiscountAmount : null,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                };

                // Thêm discount vào bảng Discounts
                _context.TbDiscounts.Add(discount);
                _context.SaveChanges();

                // Lưu các sản phẩm được chọn vào bảng DiscountProducts
                foreach (var productId in model.ProductIds)
                {
                    var discountProduct = new TbDiscountProduct
                    {
                        Id = Guid.NewGuid(),
                        DiscountId = discount.Id,
                        ProductId = productId
                    };

                    _context.TbDiscountProducts.Add(discountProduct);
                }

                _context.SaveChanges();
            }
            // Điều hướng đến trang hiển thị danh sách giảm giá hoặc thông báo thành công
            TempData["RegisterSuccess"] = "Thành công!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            var discount = _context.TbDiscounts.FirstOrDefault(d => d.Id == id);
            if (discount == null) return NotFound();

            var now = DateTime.Now;

            var selectedProductIds = _context.TbDiscountProducts
                .Where(dp => dp.DiscountId == id)
                .Select(dp => dp.ProductId)
                .ToList();

            var products = _context.TbProducts
                .Where(p => !_context.TbDiscountProducts.Any(dp =>
                    dp.ProductId == p.Id &&
                    dp.DiscountId != id && 
                    _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now)) 
                    || selectedProductIds.Contains(p.Id)) 
                .ToList();

            var categories = _context.TbCategories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            var model = new CreateDiscountViewModel
            {
                Id = discount.Id,
                Name = discount.Name,
                DiscountType = discount.DiscountType,
                DiscountValue = discount.DiscountValue,
                MaxDiscountAmount = discount.MaxDiscountAmount,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                Products = products,
                ProductIds = selectedProductIds,
                Categories = categories
            };

            return View("Detail", model);
        }
        [HttpGet]
        [Route("Discount/Create")]
        public IActionResult Create()
        {
            var now = DateTime.Now;

            var products = _context.TbProducts
                .Where(p => !_context.TbDiscountProducts.Any(dp => dp.ProductId == p.Id &&
                              _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now)))
                .ToList();

            var categories = _context.TbCategories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            var model = new CreateDiscountViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(model);
        }
        [HttpGet]
        [Route("GetDiscountedProducts")]
        public IActionResult GetDiscountedProducts(Guid? discountId = null, string productCode = "", string productName = "")
        {
            var now = DateTime.Now;

            var query = _context.TbDiscountProducts
                .Join(_context.TbDiscounts,
                    dp => dp.DiscountId,
                    d => d.Id,
                    (dp, d) => new { DiscountProduct = dp, Discount = d })
                .Join(_context.TbProducts,
                    joined => joined.DiscountProduct.ProductId,
                    p => p.Id,
                    (joined, p) => new {
                        ProductId = p.Id,
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        ProductPrice = p.Price,
                        DiscountId = joined.Discount.Id,
                        DiscountName = joined.Discount.Name,
                        DiscountType = joined.Discount.DiscountType,
                        DiscountValue = joined.Discount.DiscountValue,
                        MaxDiscountAmount = joined.Discount.MaxDiscountAmount,
                        StartDate = joined.Discount.StartDate,
                        EndDate = joined.Discount.EndDate
                    }).AsQueryable(); 

            if (discountId.HasValue && discountId != Guid.Empty)
            {
                query = query.Where(x => x.DiscountId == discountId.Value);
            }

            if (!string.IsNullOrEmpty(productCode))
            {
                query = query.Where(x => x.ProductCode.Contains(productCode));
            }

            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(x => x.ProductName.Contains(productName));
            }

            var result = query.ToList();
            return Json(result);
        }

        [HttpGet]
        [Route("GetDiscounts")]
        public IActionResult GetDiscounts()
        {
            try
            {
                var discounts = _context.TbDiscounts
                    .Select(d => new {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToList();

                return Json(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
