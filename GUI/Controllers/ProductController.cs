using GUI.Controllers.Shared;
using GUI.Models.DTOs.Product_DTO.Views;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using GUI.Shared;
using Newtonsoft.Json;
using GUI.FileBase;
using NuGet.Protocol;
using GUI.Models.DTOs.Product_DTO;

namespace GUI.Controllers
{
    public class ProductController : ControllerSharedBase
    {
        private HttpService httpService;
        public ProductController(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            httpService = new();
        }
        // GET: ProductController
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductRequest user)
        {
            try
            {
                user.Status = 1;
                user.CategoryId = Guid.Parse("B542880B-F661-456A-9ADD-265B05C1B2BB");
                user.TypeImage = "1";
                var URL = _settings.APIAddress + "api/CreateProduct/Process";
                var param = JsonConvert.SerializeObject(user);
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
                var URL = _settings.APIAddress + "api/EditUser/Process";
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
