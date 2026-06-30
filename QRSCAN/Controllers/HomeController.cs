using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models;
using System.Diagnostics;

namespace QRSCAN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Register", "Account");
            }

            ViewBag.HoTenKH = HttpContext.Session.GetString("HoTenKH");
            ViewBag.MaBan = HttpContext.Session.GetInt32("MaBan");

            var danhSachMon = await _context.MonAns
                .Include(m => m.DanhMucMon)
                .Where(m => m.TrangThai == "DangBan")
                .OrderBy(m => m.MaDanhMuc)
                .ThenBy(m => m.MaMon)
                .ToListAsync();

            return View(danhSachMon);
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