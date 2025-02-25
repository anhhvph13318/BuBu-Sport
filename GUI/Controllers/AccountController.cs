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
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly DBContext _context;
        private const int PageSize = 5; 

        public AccountController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string phoneNumber = "", int page = 1)
        {
            var query = _context.TbAccounts
                .AsNoTracking()
                .Where(a => string.IsNullOrEmpty(phoneNumber) || a.PhoneNumber.Contains(phoneNumber));

            int totalAccounts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalAccounts / PageSize);

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
                    Role = a.Role == 0 ? "Nhân viên" : "Khách hàng",
                    CreateDate = a.CreateDate
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PhoneNumber = phoneNumber;

            return View(accounts);
        }
    }
}
