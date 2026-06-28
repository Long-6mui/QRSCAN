using Microsoft.AspNetCore.Mvc;
using QRSCAN.Models;
using System.Diagnostics;

namespace QRSCAN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Register", "Account");
            }

            ViewBag.HoTenKH = HttpContext.Session.GetString("HoTenKH");
            ViewBag.MaBan = HttpContext.Session.GetInt32("MaBan");

            return View();
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