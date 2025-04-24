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
            var startDate = model.StartDate;
            var endDate = model.EndDate;

            if (endDate < startDate)
            {
                TempData["ErrorMessage"] = "Ngày kết thúc không thể trước ngày bắt đầu. Vui lòng điều chỉnh thời gian!";
                var products = _context.TbProducts.ToList();
                var categories = _context.TbCategories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList();
                var now = DateTime.Now;

                var productsInOtherDiscounts = _context.TbDiscountProducts
                    .Where(dp => (actionType != "update" || dp.DiscountId != model.Id) &&
                                _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now))
                    .Select(dp => dp.ProductId)
                    .ToList();

                model.Products = products;
                model.Categories = categories;
                model.ProductsInOtherDiscounts = productsInOtherDiscounts;

                if (actionType == "update")
                    return View("Detail", model);
                else
                    return View("Create", model);
            }

            // Danh sách sản phẩm trùng lặp
            var conflictingProducts = new List<(Guid ProductId, string ProductName, string DiscountName)>();

            foreach (var productId in model.ProductIds)
            {
                var conflicts = _context.TbDiscountProducts
                    .Where(dp => dp.ProductId == productId &&
                          (actionType != "update" || dp.DiscountId != model.Id)) // Bỏ qua chính chương trình đang cập nhật
                    .Join(_context.TbDiscounts,
                        dp => dp.DiscountId,
                        d => d.Id,
                        (dp, d) => new { Discount = d, ProductId = dp.ProductId })
                    .Where(x => (startDate <= x.Discount.EndDate && endDate >= x.Discount.StartDate)) // Kiểm tra thời gian trùng lặp
                    .Join(_context.TbProducts,
                        x => x.ProductId,
                        p => p.Id,
                        (x, p) => new { x.Discount.Name, ProductId = p.Id, ProductName = p.Name })
                    .ToList();

                if (conflicts.Any())
                {
                    foreach (var conflict in conflicts)
                    {
                        conflictingProducts.Add((conflict.ProductId, conflict.ProductName, conflict.Name));
                    }
                }
            }

            // Nếu có sản phẩm trùng lặp, trả về lỗi
            if (conflictingProducts.Any())
            {
                var uniqueConflicts = conflictingProducts.GroupBy(x => x.ProductName).Select(g => g.First()).ToList();

                string errorMessage = "Các sản phẩm sau đã thuộc chương trình khuyến mãi khác trong cùng thời gian:<br/>";
                foreach (var item in uniqueConflicts.Take(5)) // Chỉ hiển thị tối đa 5 sản phẩm trùng
                {
                    errorMessage += $"- {item.ProductName} (đã thuộc chương trình: {item.DiscountName})<br/>";
                }

                if (uniqueConflicts.Count > 5)
                {
                    errorMessage += $"... và {uniqueConflicts.Count - 5} sản phẩm khác";
                }

                // Chuẩn bị lại model để quay trở lại form với dữ liệu đã nhập
                var products = _context.TbProducts.ToList();
                var categories = _context.TbCategories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList();
                var now = DateTime.Now;

                var productsInOtherDiscounts = _context.TbDiscountProducts
                    .Where(dp => (actionType != "update" || dp.DiscountId != model.Id) &&
                                _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now))
                    .Select(dp => dp.ProductId)
                    .ToList();

                model.Products = products;
                model.Categories = categories;
                model.ProductsInOtherDiscounts = productsInOtherDiscounts;

                TempData["ErrorMessage"] = errorMessage;

                if (actionType == "update")
                    return View("Detail", model);
                else
                    return View("Create", model);
            }


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

            var products = _context.TbProducts.ToList();

            var productsInOtherDiscounts = _context.TbDiscountProducts
                .Where(dp => dp.DiscountId != id &&
                             _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now))
                .Select(dp => dp.ProductId)
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
                Categories = categories,
                ProductsInOtherDiscounts = productsInOtherDiscounts
            };

            return View("Detail", model);
        }
        [HttpGet]
        [Route("Discount/Create")]
        public IActionResult Create()
        {
            var now = DateTime.Now;

            var products = _context.TbProducts.ToList();

            var productsInOtherDiscounts = _context.TbDiscountProducts
                .Where(dp => _context.TbDiscounts.Any(d => d.Id == dp.DiscountId && d.EndDate >= now))
                .Select(dp => dp.ProductId)
                .ToList();

            var categories = _context.TbCategories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            var model = new CreateDiscountViewModel
            {
                Products = products,
                Categories = categories,
                ProductsInOtherDiscounts = productsInOtherDiscounts
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
        [HttpGet]
        [Route("GetProductDiscountInfo")]
        public IActionResult GetProductDiscountInfo(Guid productId)
        {
            var discountInfo = _context.TbDiscountProducts
                .Where(dp => dp.ProductId == productId)
                .Join(_context.TbDiscounts,
                    dp => dp.DiscountId,
                    d => d.Id,
                    (dp, d) => new {
                        DiscountId = d.Id,
                        DiscountName = d.Name,
                        StartDate = d.StartDate,
                        EndDate = d.EndDate
                    })
                .ToList();

            return Json(discountInfo);
        }
    }
}
