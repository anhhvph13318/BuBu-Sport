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

namespace GUI.Controllers
{
    [Authorize(Roles = "Admin")]
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
        // GET: ProductController
        //[AllowAnonymous]
        public async Task<ActionResult> Index(string s)
        {
            var obj = new GetListProductRequest();
            var model = new IndexObject();
            obj.Name = string.IsNullOrEmpty(s) ? "" : s;
            var URL = _settings.APIAddress + "api/HomePage/Process";
            var param = JsonConvert.SerializeObject(obj);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();

            model.Data = result.Data;

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
        private async Task<IEnumerable<CategoryDto>> FetchCategory()
        {
            return await _context.TbCategories.AsNoTracking()
                .Select(e => new CategoryDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Status = (int)e.Status!,
                    CreateDate = e.CreateDate
                })
                .OrderBy(e => e.CreateDate)
                .ToListAsync();
        }
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductRequest product)
        {
            if (!ModelState.IsValid)
            {
                var categories = await FetchCategory();
                ViewBag.Categories = categories;
                return View(product);
            }

            try
            {
                Random random = new Random();
                string randomTwoDigits = random.Next(10, 100).ToString();
                product.Code = "SP" + randomTwoDigits;
                product.Status = 1;
                product.TypeImage = "1";

                var URL = _settings.APIAddress + "api/CreateProduct/Process";
                var param = JsonConvert.SerializeObject(product);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();

                if (result.Status == "400")
                {
                    ModelState.AddModelError(string.Empty, result.Messages.FirstOrDefault()?.MessageText ?? "Có lỗi xảy ra.");
                    var categories = await FetchCategory();
                    ViewBag.Categories = categories;
                    return View(product);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var categories = await FetchCategory();
                ViewBag.Categories = categories;
                ModelState.AddModelError(string.Empty, "Có lỗi trong quá trình xử lý.");
                return View(product);
            }
        }
 

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var categories = await FetchCategory();
            ViewBag.Categories = categories;
            var URL = _settings.APIAddress + "api/DetailProduct/Process";
            var req = new DetailProductRequest() { ID = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailProductResponse>>(res) ?? new();
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
    }
}
