using GUI.Controllers.Shared;
using GUI.Models.DTOs;
using GUI.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GUI.Shared;
using DATN_ACV_DEV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GUI.Controllers
{
    [Controller]
    [Route("customers")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : ControllerSharedBase
    {
        private HttpService httpService;
        DBContext dBContext;
        public CustomerController(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            httpService = new();
            dBContext = new DBContext();
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var customers = await dBContext.TbCustomers
                .Select(c => new CustomerDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Address = c.Adress,
                    Status = c.Status,
                    Point = c.Point
                })
                .ToListAsync();

            int totalRecords = customers.Count();
            var paginatedCustomers = customers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;

            return View(paginatedCustomers);
        }

    }
}
