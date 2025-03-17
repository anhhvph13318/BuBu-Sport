using DATN_ACV_DEV.Entity;
using GUI.Controllers.Shared;
using GUI.Shared;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GUI.Models.DTOs.ProductDetail_DTO;
using GUI.FileBase;
using Microsoft.EntityFrameworkCore;
using GUI.Models.DTOs;

namespace GUI.Controllers
{
    public class ProductDetailController : ControllerSharedBase
    {
        private readonly DBContext _context;
        private HttpService httpService;
        public ProductDetailController(IOptions<CommonSettings> settings, DBContext context)
        {
            _settings = settings.Value;
            httpService = new();
            _context = context;
        }
        [Route("/CreateDetail")]
        public async Task<ActionResult> CreateDetail(int ProductId)
        {
            var colors = await FetchColor();
            ViewBag.Colors = colors;
            var sizes = await FetchSize();
            ViewBag.Sizes = sizes;
            ViewBag.ProductId = ProductId;
            return View();
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
        [HttpPost]
        [Route("CreateDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(CreateProductDetailRequest product)
        {
            try
            {
                var URL = _settings.APIAddress + "api/CreateProductDetail/Process";
                var param = JsonConvert.SerializeObject(product);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductDetailResponse>>(res) ?? new();
                if (result.Status == "400")
                {
                    ModelState.AddModelError("UserName", result.Messages.FirstOrDefault().MessageText);
                    return Empty;
                }
                return Redirect($"/Product/Edit/{product.ProductID}");

            }
            catch
            {
                return View();
            }
        }
    }
}
