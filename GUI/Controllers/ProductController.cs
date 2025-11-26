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
using System.Net.WebSockets;
using ExcelDataReader;

namespace GUI.Controllers
{
    //[Authorize(Roles = "Admin,Employee")]
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

            // Xử lý khuyến mãi 
            var promotion = _context.TbDiscountProducts.ToList();
            foreach (var item in promotion)
            {
                var discount = _context.TbDiscounts
                    .Where(c => c.Id == item.DiscountId
                           && c.StartDate <= DateTime.Now
                           && c.EndDate >= DateTime.Now) 
                    .Select(c => c.DiscountValue)
                    .FirstOrDefault();
                if (discount != null)
                {
                    foreach (var item1 in model.Data.LstProduct)
                    {
                        if (item.ProductId == item1.Id)
                        {
                            item1.PriceSale = Convert.ToDecimal(item1.Price - (item1.Price * discount / 100));
                        }
                    }
                }
            }

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
            var Colors = await FetchColor();
            ViewBag.Colors = Colors;
            var Sizes = await FetchSize();
            ViewBag.Sizes = Sizes;
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
        private async Task<List<ProductImportModel>> ReadExcelFile(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var products = new List<ProductImportModel>();

            using (var stream = file.OpenReadStream())
            using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();

                var table = result.Tables[0]; // Sheet đầu tiên

                for (int i = 1; i < table.Rows.Count; i++) // Bỏ row header
                {
                    var row = table.Rows[i];

                    var p = new ProductImportModel
                    {
                        Name = row[0].ToString(),
                        Code = row[1].ToString(),
                        CategoryId = row[2].ToString(),
                        Color = row[3].ToString(),
                        Description = row[4].ToString(),
                        Price = Convert.ToDecimal(row[5]),
                        UrlImage = row[6].ToString(),

                        XS = Convert.ToInt32(row[7]),
                        S = Convert.ToInt32(row[8]),
                        M = Convert.ToInt32(row[9]),
                        L = Convert.ToInt32(row[10]),
                        XL = Convert.ToInt32(row[11]),
                        XXL = Convert.ToInt32(row[12]),
                    };

                    products.Add(p);
                }
            }

            return products;
        }
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel.");

            // Bước 2: Đọc file Excel
            var productList = await ReadExcelFile(file);

            // TODO: Lưu dữ liệu vào DB 
            CreateProductRequest product = new CreateProductRequest();
            List<SizeQuantityDto> lstSize = new List<SizeQuantityDto>();
            SizeQuantityDto size = new SizeQuantityDto();
            foreach (var item in productList)
            {
                if (item.XS != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "XS").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.XS;
                    lstSize.Add(size);
                }
                if (item.S != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "S").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.S;
                    lstSize.Add(size);
                }
                if (item.L != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "L").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.L;
                    lstSize.Add(size);
                }
                if (item.M != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "M").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.M;
                    lstSize.Add(size);
                }
                if (item.XXL != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "XXL").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.XXL;
                    lstSize.Add(size);
                }
                if (item.XL != 0)
                {
                    size = new SizeQuantityDto();
                    size.IdSize = _context.TbSizes.Where(c => c.SizeName == "XL").Select(c => c.Id).FirstOrDefault();
                    size.QuantitySize = item.XL;
                    lstSize.Add(size);
                }
            }
            foreach (var item in productList)
            {
                product.Name = item.Name;
                product.Code = "SP" + item.Code;
                product.Color = _context.TbColors.Where(c => c.Name == item.Color).Select(c => c.Id).FirstOrDefault();
                product.CategoryId = _context.TbCategories.Where(c => c.Name == item.CategoryId).Select(c => c.Id).FirstOrDefault();
                product.Price = item.Price;
                product.Description = item.Description;
                product.SizesQuantities = lstSize;
                product.UrlImage = item.UrlImage;
                // SaveToDatabase(productList);
                if (_context.TbProducts.Where(c=>c.Code != product.Code).Select(c=>c.Code).FirstOrDefault().Count() == 0)
                {
                    Create(product);
                }
                else {
                    foreach (var item1 in product.SizesQuantities)
                    {
                        var colorId = product.Color;
                        var sizeId = item1.IdSize;
                        var productId = _context.TbProducts.Where(c => c.Code == product.Code).Select(c => c.Id).FirstOrDefault();
                        var tbProductDetail = _context.TbProductDetails.Where(c => c.ColorId == colorId && c.SizeId == sizeId && c.ProductId == productId).FirstOrDefault();
                        if (tbProductDetail == null)
                        {
                            var tbProductDetails = new TbProductDetail();
                            tbProductDetails.Id = Guid.NewGuid();
                            tbProductDetails.Price = item.Price;
                            tbProductDetails.Quantity = item1.QuantitySize;
                            tbProductDetails.ImageId = _context.TbImages.Where(c => c.Url == item.UrlImage).Select(c => c.Id).FirstOrDefault();
                            tbProductDetails.ColorId = colorId;
                            tbProductDetails.SizeId = sizeId;
                            tbProductDetails.ProductId = productId;
                            tbProductDetails.CreateDate = DateTime.Now;
                            _context.TbProductDetails.Add(tbProductDetails);
                            _context.SaveChanges();
                        }
                        else
                        {
                            tbProductDetail.Quantity += item1.QuantitySize;
                            _context.SaveChanges();
                        }
                    }
                }
            }

            TempData["success"] = "Import thành công!";
            return RedirectToAction("Index");
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
                return Redirect("http://localhost:5011/product");
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
            var datadetail = _context.TbProductDetails
                .Where(c => c.ProductId == result.Data.Id)
                .ToList();

            var groupdata = datadetail
                .GroupBy(c => new { c.ColorId, c.SizeId })
                .Select(d => new
                {
                    ProductDetail = d,
                    SizeCreateDate = _context.TbSizes
                        .Where(s => s.Id == d.Key.SizeId)
                        .Select(s => s.CreateDate)
                        .FirstOrDefault()
                })
                .Select(d => new TbProductDetail
                {
                    Id = d.ProductDetail.First().Id,
                    Price = d.ProductDetail.First().Price, // Giữ nguyên giá của bản ghi đầu tiên
                    Quantity = d.ProductDetail.Sum(p => p.Quantity), // Cộng tổng số lượng
                    ImageId = d.ProductDetail.Select(x => x.ImageId).Distinct().Count() == 1
                        ? d.ProductDetail.First().ImageId
                        : d.ProductDetail.OrderByDescending(x => x.CreateDate).First().ImageId,
                    ColorId = d.ProductDetail.Key.ColorId,
                    SizeId = d.ProductDetail.Key.SizeId,
                    ProductId = d.ProductDetail.First().ProductId,
                    CreateDate = d.SizeCreateDate // Lấy CreateDate từ bảng Size
                })
                .ToList();

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
                SizeName = d.SizeId.HasValue && sizeDict.ContainsKey(d.SizeId.Value) ? sizeDict[d.SizeId.Value] : "Unknown",
                CreateDate = d.CreateDate,
            }).OrderByDescending(c=>c.CreateDate).ToList();
            foreach (var item in result.Data.DetailDataFinal)
            {
                item.UrlImage = _context.TbImages.Where(c => c.Id == item.ImageId).Select(c=>c.Url).FirstOrDefault();
            }
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
        [HttpPost]
        public IActionResult UpdateDetailQuantity([FromBody] UpdateDetailQuantityModel model)
        {
            var detail = _context.TbProductDetails.FirstOrDefault(d => d.Id == model.Id);
            if (detail != null)
            {
                detail.Quantity = model.Quantity;
                _context.SaveChanges();
                var productDetail = _context.TbProductDetails.Where(d => d.ProductId == detail.ProductId).ToList();
                var product = _context.TbProducts.Where(c => c.Id == detail.ProductId).FirstOrDefault();
                product.Quantity = productDetail.Sum(d => d.Quantity);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public class UpdateDetailQuantityModel
        {
            public Guid Id { get; set; }
            public int Quantity { get; set; }
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

        [HttpGet]
        [Route("api/products")]
        public async Task<IActionResult> GetProducts()
        {
            var lstprice = new decimal();
            TbOrderDetail tbOrderDetails = new TbOrderDetail();
            var products = await _context.TbProducts.Where(c=>c.IsDelete != true)
                .Select(p => new ProductOFFDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.TbProductDetails.FirstOrDefault().Image.Url
                })
                .ToListAsync();
            foreach (var product in products)
            {
                var lstDiscountId = _context.TbDiscountProducts.Where(c => c.ProductId == product.Id).Select(c=>c.DiscountId).ToList();
                foreach (var item in lstDiscountId)
                {
                    var discount = _context.TbDiscounts.Where(c=>c.Id == item && c.EndDate >= DateTime.Now).Select(c=>c.DiscountValue).FirstOrDefault();
                    if (discount != null)
                    {
                        product.Price = Convert.ToDecimal(product.Price * (1 - discount / 100));
                    }
                }
            }
            return Ok(products);
        }
        [HttpGet]
        [Route("api/products/{productId}/variants")]
        public async Task<IActionResult> GetProductVariants(Guid productId)
        {
            var variants = await _context.TbProductDetails
                .Where(pd => pd.ProductId == productId)
                .Include(pd => pd.Color) 
                .Include(pd => pd.Size) 
                .Select(pd => new
                {
                    pd.Id,
                    Color = pd.Color.Name,
                    Size = pd.Size.SizeName,
                    pd.Price,
                    pd.Quantity,
                    ImageUrl = pd.Image.Url 
                })
                .ToListAsync();

            return Ok(variants);
        }
        [HttpGet("/api/productdetails/{id}")]
        public async Task<IActionResult> GetProductDetail(Guid id)
        {
            var productDetail = await _context.TbProductDetails
                .Where(pd => pd.Id == id)
				.Select(pd => new ProductDetailDTO
				{
					Id = pd.Id,
                    Name = _context.TbProducts.Where(a => a.Id == _context.TbProductDetails.Where(c => c.Id == pd.Id).Select(c => c.ProductId).FirstOrDefault()).Select(a => a.Name).FirstOrDefault(),
					Color = pd.Color.Name,
					Size = pd.Size.SizeName,
					Price = _context.TbProducts.Where(a=>a.Id == _context.TbProductDetails.Where(c=>c.Id == pd.Id).Select(c=>c.ProductId).FirstOrDefault()).Select(a=>a.Price).FirstOrDefault(),
					Quantity = pd.Quantity,
					ImageUrl = pd.Image.Url,
					ProductId = pd.ProductId,
                    ProductCode = _context.TbProducts.Where(a => a.Id == _context.TbProductDetails.Where(c => c.Id == pd.Id).Select(c => c.ProductId).FirstOrDefault()).Select(a => a.Code).FirstOrDefault(),

				})
				.FirstOrDefaultAsync();

            if (productDetail == null)
            {
                return NotFound();
            }
            decimal? discount = 0m;
            var discountavai = _context.TbDiscountProducts.Where(a => a.ProductId == productDetail.ProductId).ToList();
            foreach (var item in discountavai)
            {
                var discountDate = _context.TbDiscounts.Where(c => c.Id == item.DiscountId).FirstOrDefault();
                if (discountDate.EndDate >= DateTime.Now)
                {
                    discount = discountDate.DiscountValue;
                }
            }
            if (discount != null)
            {
                productDetail.Price = Convert.ToDecimal(productDetail.Price * (1 - discount / 100));
            }
            
            return Ok(productDetail);
        }
		public class ProductDetailDTO
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string ProductCode { get; set; }
			public string Color { get; set; }       // Tên màu
			public string Size { get; set; }        // Tên kích thước
			public decimal Price { get; set; }
			public int Quantity { get; set; }
			public string ImageUrl { get; set; }    // Đường dẫn hình ảnh
			public Guid? ProductId { get; set; }
		}
	}
}
