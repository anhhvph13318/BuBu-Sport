using GUI.Controllers.Shared;
using GUI.Models.DTOs.Product_DTO.Views;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GUI.Shared;
using Newtonsoft.Json;
using GUI.FileBase;
using GUI.Models.DTOs.Product_DTO;
using Microsoft.AspNetCore.Authorization;
using GUI.Models.DTOs;
using DATN_ACV_DEV.Entity;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace GUI.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ProductController : ControllerSharedBase
    {
        private readonly DBContext _context;
        private HttpService httpService;
        public ProductController(IOptions<CommonSettings> settings, DBContext context)
        {
            _settings = settings.Value;
            httpService = new();
            _context = context;
        }

        [Route("/product")]
        public async Task<ActionResult> Index(string? name = "", Guid? categoryId = null, decimal? priceFrom = null, decimal? priceTo = null, int status = 0)
        {
            var obj = new GetListProductRequest();
            var model = new IndexObject();

            // Thiết lập các tham số tìm kiếm
            obj.Name = string.IsNullOrEmpty(name) ? "" : name;
            obj.CategoryID = categoryId;
            obj.PriceFrom = priceFrom;
            obj.PriceTo = priceTo;
            obj.Status = status > 0 ? status : null;

            var URL = _settings.APIAddress + "api/HomePage/Process";
            var param = JsonConvert.SerializeObject(obj);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();

            model.Data = result.Data;
            var checkname = 0;
            // Truyền các giá trị tìm kiếm hiện tại vào ViewBag để hiển thị trên giao diện
            if (Guid.TryParse(name, out Guid myGuid))
            {
                checkname = 1;
            }
            ViewBag.CurrentName = checkname == 0 ? name : "";
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentPriceFrom = priceFrom;
            ViewBag.CurrentPriceTo = priceTo;
            ViewBag.CurrentStatus = status;

            // Tải danh sách danh mục để sử dụng trong dropdown filter
            var categories = await FetchCategory();
            ViewBag.Categories = categories;

            return View(model);
        }

        // GET: ProductController/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        public async Task<ActionResult> Create()
        {
            var categories = await FetchCategory();
            ViewBag.Categories = categories;
            return View();
        }
        private async Task<IEnumerable<CategoryDTO>> FetchCategory()
        {
            return await _context.TbCategories.AsNoTracking()
                .Select(e => new CategoryDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Status = (int)e.Status!,
                    CreateDate = e.CreateDate
                })
                .OrderBy(e => e.CreateDate)
                .ToListAsync();
        }
        private async Task<IEnumerable<ColorDTO>> FetchColor()
        {
            return await _context.TbColors.AsNoTracking()
                .Select(e => new ColorDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Status = (int)e.Status!,
                    CreateDate = e.CreateDate
                })
                .OrderBy(e => e.CreateDate)
                .ToListAsync();
        }
        private async Task<IEnumerable<SizeDTO>> FetchSize()
        {
            return await _context.TbSizes.AsNoTracking()
                .Select(e => new SizeDTO
                {
                    Id = e.Id,
                    SizeName = e.SizeName,
                })
                .ToListAsync();
        }
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductRequest product)
        {
            try
            {
                product.Status = 1;
                product.TypeImage = "1";
                var URL = _settings.APIAddress + "api/CreateProduct/Process";
                var param = JsonConvert.SerializeObject(product);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();
                if (result.Status == "400")
                {
                    ModelState.AddModelError("UserName", result.Messages.FirstOrDefault().MessageText);
                    return Empty;
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var colors = await FetchColor();
            ViewBag.Colors = colors;
            var categories = await FetchCategory();
            ViewBag.Categories = categories;
            var sizes = await FetchSize();
            ViewBag.Sizes = sizes;
            var URL = _settings.APIAddress + "api/DetailProduct/Process";
            var req = new DetailProductRequest() { ID = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailProductResponse>>(res) ?? new();
            // Lấy danh sách chi tiết sản phẩm từ database
            var datadetail = _context.TbProductDetails.Where(c => c.ProductId == result.Data.Id).ToList();
            var groupdata = datadetail
                .GroupBy(c => new {c.ColorId,c.SizeId})
                .Select(d => new TbProductDetail
                {
                    Id = d.First().Id,
                    Price = d.First().Price, // Giữ nguyên giá của bản ghi đầu tiên
                    Quantity = d.Sum(p => p.Quantity), // Cộng tổng số lượng
                    ImageId = d.First().ImageId,
                    ColorId = d.Key.ColorId,
                    SizeId = d.Key.SizeId,
                    ProductId = d.First().ProductId
                }).ToList();
            result.Data.DetailData = groupdata;

            // Tạo Dictionary để tối ưu truy vấn
            var colorDict = colors.ToDictionary(c => c.Id, c => c.Name);
            var sizeDict = sizes.ToDictionary(s => s.Id, s => s.SizeName);

            // Chuyển đổi danh sách `TbProductDetail` thành `TestDame`
            result.Data.DetailDataFinal = groupdata.Select(d => new TestDame
            {
                Id = d.Id,
                Price = d.Price,
                Quantity = d.Quantity,
                ImageId = d.ImageId,
                ColorId = d.ColorId,
                SizeId = d.SizeId,
                ProductId = d.ProductId,
                ColorName = d.ColorId.HasValue && colorDict.ContainsKey(d.ColorId.Value) ? colorDict[d.ColorId.Value] : "Unknown",
                SizeName = d.SizeId.HasValue && sizeDict.ContainsKey(d.SizeId.Value) ? sizeDict[d.SizeId.Value] : "Unknown"
            }).ToList();
            var model = result.Data;
            return View(model);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProductRequest user)
        {
            try
            {
                var URL = _settings.APIAddress + "api/EditProduct/Process";
                var param = JsonConvert.SerializeObject(user);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<DetailProductResponse>>(res) ?? new();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var URL = _settings.APIAddress + "api/DetailProduct/Process";
            var req = new DetailProductRequest() { ID = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailProductResponse>>(res) ?? new();
            var model = result.Data;
            return View(model);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var URL = _settings.APIAddress + "api/DeleteProduct/Process";
                var req = new DetailProductRequest() { ID = id };
                var param = JsonConvert.SerializeObject(req);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult CheckProductCode(string code)
        {
            bool isAvailable = !_context.TbProducts.Any(p => p.Code == code);
            return Json(new { isAvailable });
        }
    }
}
