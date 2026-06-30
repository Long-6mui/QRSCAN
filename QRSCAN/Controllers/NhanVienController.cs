using Microsoft.AspNetCore.Mvc;

namespace QRSCAN.Controllers
{
    public class NhanVienController : Controller
    {
        public IActionResult Index()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.MaNV = maNV;
            ViewBag.MaVT = HttpContext.Session.GetInt32("MaVT");
            ViewBag.HoTenNV = HttpContext.Session.GetString("HoTenNV");
            ViewBag.TenVaiTro = HttpContext.Session.GetString("TenVaiTro");

            return View();
        }
    }
}