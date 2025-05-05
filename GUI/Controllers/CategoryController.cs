using Azure.Core;
using DATN_ACV_DEV.Entity;
using GUI.Models.DTOs;
using GUI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using GUI.Models.DTOs.Product_DTO;
using GUI.Models.DTOs.Product_DTO.Views;
using GUI.Shared.Common;
using Microsoft.Extensions.Options;
using GUI.Controllers.Shared;
using GUI.FileBase;

namespace GUI.Controllers;

[Controller]
[Route("categories")]
[Authorize(Roles = "Admin")]
public class CategoryController : ControllerSharedBase
{
    private readonly DBContext _context;
    private readonly UserSession _session;
    private HttpService httpService;

    public CategoryController(DBContext context, UserSession session, IOptions<CommonSettings> settings)
    {
        _context = context;
        _session = session;
        _settings = settings.Value;
        httpService = new();
    }

    public async Task<IActionResult> Index()
    {
        var categories = await FetchCategory();

        return View(categories);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] string id)
    {
        var category = await _context.TbCategories
            .Select(e => new CategoryDTO
            {
                Id = e.Id,
                Name = e.Name,
                Status = (int)e.Status!,
                Description = "update"
            })
            .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));
        if (category is null) return BadRequest();

        return Json(new
        {
            Modal = await RenderViewAsync("_CategoryModal", category)
        });
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CategoryDTO request)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(e => e.Name == request.Name);
        if (category is not null)
            return BadRequest();

        category = new TbCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = (int)request.Status,
            CreateBy = Guid.NewGuid(),
            CreateDate = DateTime.Now,
        };

        await _context.TbCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        var categories = await FetchCategory();
        return Json(new
        {
            Table = await RenderViewAsync("_CategoryTable", categories),
            Modal = await RenderViewAsync("_CategoryModal", new CategoryDTO())
        });
    }
    //update category
    [HttpPatch("create/{id}")]
    public async Task<IActionResult> Update([FromBody] CategoryDTO request, [FromRoute] string id)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));
        if(category is null) return BadRequest();

        if(category.Name == request.Name && Guid.Parse(id) != category.Id) return BadRequest();

        category.Name = request.Name;
        category.Status = (int)request.Status;
        category.UpdateDate = DateTime.Now;
        category.UpdateBy = Guid.NewGuid();

        await _context.SaveChangesAsync();

        var categories = await FetchCategory();
        return Json(new
        {
            Message = "Đăng ký thành công!",
            Table = await RenderViewAsync("_CategoryTable", categories),
            Modal = await RenderViewAsync("_CategoryModal", new CategoryDTO())
        });
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] string name = "", int status = 0)
    {     
        var categories = await _context.TbCategories.AsNoTracking()
            .Where(e => e.Name.StartsWith(name) && e.Status == status)
            .Select(e => new CategoryDTO
            {
                Id = e.Id,
                Name = e.Name,
                Status = (int)e.Status!,
                CreateDate = e.CreateDate
            })
            .OrderBy(e => e.CreateDate)
            .ToListAsync();
        if (status == 2)
        {
            categories = await _context.TbCategories.AsNoTracking()
            .Where(e => e.Name.StartsWith(name))
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
        return Json(new
        {
            Table = await RenderViewAsync("_CategoryTable", categories)
        });
    }
    [HttpGet]
    [Route("produtCategory")]
    public async Task<ActionResult> ProdutCategory(Guid s)
    {
        var obj = new GetListProductRequest();
        var model = new IndexObject();
        obj.CategoryID = s;
        var URL = _settings.APIAddress + "api/HomePage/Process";
        var param = JsonConvert.SerializeObject(obj);
        var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
        var result = JsonConvert.DeserializeObject<BaseResponse<GetListProductResponse>>(res) ?? new();

        model.Data = result.Data;

        return View(model);
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

    private async Task<string> RenderViewAsync(string viewName, object? model)
    {

        ViewData.Model = model;

        using var writer = new StringWriter();
        IViewEngine viewEngine = HttpContext.RequestServices.GetService<ICompositeViewEngine>()!;
        ViewEngineResult viewResult = viewEngine!.FindView(ControllerContext, viewName, false);

        if (viewResult.Success == false)
        {
            return $"A view with the name {viewName} could not be found";
        }

        ViewContext viewContext = new(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return writer.GetStringBuilder().ToString();
    }
    //
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetCategoryList()
    {
        var categories = await FetchCategory();
        return Json(categories);
    }
}
