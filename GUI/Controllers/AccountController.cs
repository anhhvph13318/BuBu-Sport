using DATN_ACV_DEV.Entity;
using GUI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GUI.Controllers
{
    [Controller]
    [Route("accounts")]
    //[Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly DBContext _context;
        private const int PageSize = 5;

        public AccountController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string phoneNumber = "", int page = 1, string role = "", string status = "")
        {
            var query = _context.TbAccounts
                .AsNoTracking()
                .Where(a =>
                    (string.IsNullOrEmpty(phoneNumber) || a.PhoneNumber.Contains(phoneNumber)) &&
                    (string.IsNullOrEmpty(role) || (a.Role == 0 && role == "Khách hàng") || (a.Role == 1 && role == "Nhân viên"))
                );

            var accounts = await query
                .OrderBy(a => a.CreateDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new AccountDTO
                {
                    Id = a.Id,
                    AccountCode = a.AccountCode,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    Role = a.Role == 0 ? "Khách hàng" : "Nhân viên",
                    Status = a.CustomerId != null
                        ? _context.TbCustomers.Where(c => c.Id == a.CustomerId).Select(c => c.Status).FirstOrDefault()
                        : _context.TbUsers.Where(c => c.Id == a.EmployeeId).Select(c => c.InActive == true ? "Không hoạt động" : "Đang hoạt động").FirstOrDefault(),

                    CreateDate = a.CreateDate
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)await query.CountAsync() / PageSize);
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.Role = role;
            ViewBag.Status = status;

            return View(accounts);
        }
    }
}