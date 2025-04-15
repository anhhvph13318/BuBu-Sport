using DATN_ACV_DEV.Entity;
using GUI.Controllers.Shared;
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
        public async Task<IActionResult> Index()
        {
            var Discounts = _context.TbDiscounts.ToList();

            // Trả về View với Model là danh sách sản phẩm
            var model = new DiscountListViewModel
            {
                Discounts = Discounts
            };
            return View(model);
        }
        [HttpPost]
        [Route("CreateDiscount")]
        public IActionResult CreateOrUpdateDiscount(CreateDiscountViewModel model, string actionType)
        {
            if (actionType == "update")
            {
                TbDiscount tbDiscount = new TbDiscount();
                tbDiscount = _context.TbDiscounts.Where(c => c.Id == model.Id).FirstOrDefault();
                tbDiscount.Name = model.Name != null ? model.Name : tbDiscount.Name;
                tbDiscount.DiscountType = model.DiscountType != null ? model.DiscountType : tbDiscount.DiscountType;
                tbDiscount.DiscountValue = model.DiscountValue != null ? model.DiscountValue : tbDiscount.DiscountValue;
                tbDiscount.MaxDiscountAmount = model.MaxDiscountAmount != null ? model.MaxDiscountAmount : tbDiscount.MaxDiscountAmount;
                tbDiscount.StartDate = model.StartDate != null ? model.StartDate : tbDiscount.StartDate;
                tbDiscount.EndDate = model.EndDate != null ? model.EndDate : tbDiscount.EndDate;
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
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            var discount = _context.TbDiscounts.FirstOrDefault(d => d.Id == id);
            if (discount == null) return NotFound();

            var selectedProductIds = _context.TbDiscountProducts
                .Where(dp => dp.DiscountId == id)
                .Select(dp => dp.ProductId)
                .ToList();

            var products = _context.TbProducts.ToList();

            var model = new CreateDiscountViewModel
            {
                Name = discount.Name,
                DiscountType = discount.DiscountType,
                DiscountValue = discount.DiscountValue,
                MaxDiscountAmount = discount.MaxDiscountAmount,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                Products = products,
                ProductIds = selectedProductIds
            };

            return View("Create", model); // Có thể dùng lại view tạo discount
        }
        [HttpGet]
        [Route("Discount/Create")]
        public IActionResult Create()
        {
            var productIdList = _context.TbDiscountProducts.Select(c => c.ProductId).ToList();

            var products = _context.TbProducts
                .Where(p => !productIdList.Contains(p.Id))
                .ToList();

            // Trả về View với Model là danh sách sản phẩm
            var model = new CreateDiscountViewModel
            {
                Products = products
            };

            return View(model);
        }
    }
}
