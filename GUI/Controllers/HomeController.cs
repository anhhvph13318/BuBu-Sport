using GUI.Controllers.Shared;
using GUI.Models;
using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GUI.Controllers
{
    public class HomeController : ControllerSharedBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_settings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
