using Microsoft.AspNetCore.Mvc;

namespace GUI.Controllers
{
	public class StorefrontController : Controller
	{
		[Route("/")]
		public IActionResult Index()
		{
			return View();
		}

		[Route("/Store")]
		public IActionResult Store()
		{
			return View();
		}
	}
}
