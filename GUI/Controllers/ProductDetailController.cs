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
using System.Drawing.Imaging;

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
        public async Task<ActionResult> CreateDetail(Guid ProductId)
        {
            var colors = await FetchColor();
            ViewBag.Colors = colors;
            var sizes = await FetchSize();
            ViewBag.Sizes = sizes;
            ViewBag.Color = sizes;
            ViewBag.ProductId = ProductId;
            var entity = _context.TbProductDetails.Where(c => c.Id == ProductId).FirstOrDefault();
            var model = new CreateProductDetailRequest
            {
                Color = entity.ColorId,
                UrlImage = _context.TbImages.Where(c=>c.Id == entity.ImageId).Select(c=>c.Url).FirstOrDefault(),
                SizesQuantities = new List<SizeQuantityDto>
    {
                new SizeQuantityDto
                {
                    IdSize = (Guid)entity.SizeId,
                    QuantitySize = entity.Quantity // giả sử entity có Quantity
                }
    }
                // map các thuộc tính khác nếu cần
            };
            return View(model);
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
                    CreateDate = e.CreateDate
                })
                .OrderBy(e => e.CreateDate)
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
        public class ColorDto
        {
            public Guid ProductID { get; set; }
            public string Name { get; set; }
        }
        public class SizeDto
        {
            public Guid ProductID { get; set; }
            public string SizeName { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> CreateColorDetail([FromBody] ColorDto colorDto)
        {
            try
            {
                var existingColor = await _context.TbColors
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == colorDto.Name.ToLower());

                if (existingColor != null)
                {
                    return BadRequest(new { success = false, message = "Màu sắc này đã tồn tại." });
                }

                var URL = _settings.APIAddress + "api/CreateColor/Process";
                var param = JsonConvert.SerializeObject(colorDto);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductDetailResponse>>(res) ?? new();

                if (result.Status == "400")
                {
                    return BadRequest(new { success = false, message = result.Messages.FirstOrDefault()?.MessageText ?? "Lỗi không xác định" });
                }

                var redirectUrl = $"http://localhost:5011/CreateDetail?productId={colorDto.ProductID}";
                return Ok(new { success = true, url = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateSizeDetail([FromBody] SizeDto sizeDto)
        {
            try
            {
                var URL = _settings.APIAddress + "api/CreateSize/Process";
                var param = JsonConvert.SerializeObject(sizeDto);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductDetailResponse>>(res) ?? new();

                if (result.Status == "400")
                {
                    return BadRequest(new { success = false, message = result.Messages.FirstOrDefault()?.MessageText ?? "Lỗi không xác định" });
                }

                var redirectUrl = $"http://localhost:5011/CreateDetail?productId={sizeDto.ProductID}";
                return Ok(new { success = true, url = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại!" });
            }
        }

    }
}
