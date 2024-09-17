using Azure.Core;
using DATN_ACV_DEV.Entity;
using GUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace GUI.Controllers;

[Controller]
[Route("categories")]
public class CategoryController : Controller
{
    private readonly DBContext _context;
    private readonly UserSession _session;

    public CategoryController(DBContext context, UserSession session)
    {
        _context = context;
        _session = session;
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
            .Select(e => new CategoryDto
            {
                Id = e.Id,
                Name = e.Name,
                Status = (int)e.Status!
            })
            .FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));
        if (category is null) return BadRequest();

        return Json(new
        {
            Modal = await RenderViewAsync("_CategoryModal", category)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryDto request)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(e => e.Name == request.Name);
        if (category is not null)
            return BadRequest();

        category = new TbCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = (int)request.Status,
            CreateBy = _session.UserId,
            CreateDate = DateTime.Now,
        };

        await _context.TbCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        var categories = await FetchCategory();
        return Json(new
        {
            Table = await RenderViewAsync("_CategoryTable", categories),
            Modal = await RenderViewAsync("_CategoryModal", new CategoryDto())
        });
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromBody] CategoryDto request, [FromRoute] string id)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));
        if(category is null) return BadRequest();

        if(category.Name == request.Name) return BadRequest();

        category.Name = request.Name;
        category.Status = (int)request.Status;
        category.UpdateDate = DateTime.Now;
        category.UpdateBy = _session.UserId;

        await _context.SaveChangesAsync();

        var categories = await FetchCategory();
        return Json(new
        {
            Table = await RenderViewAsync("_CategoryTable", categories),
            Modal = await RenderViewAsync("_CategoryModal", new CategoryDto())
        });
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] string name = "")
    {
        var categories = await _context.TbCategories.AsNoTracking()
            .Where(e => e.Name.StartsWith(name))
            .Select(e => new CategoryDto
            {
                Id = e.Id,
                Name = e.Name,
                Status = (int)e.Status!,
                CreateDate = e.CreateDate
            })
            .OrderBy(e => e.CreateDate)
            .ToListAsync();

        return Json(new
        {
            Table = await RenderViewAsync("_CategoryTable", categories)
        });
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
}
