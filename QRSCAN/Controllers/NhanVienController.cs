using Microsoft.AspNetCore.Mvc;

namespace QRSCAN.Controllers
{
    public class NhanVienController : Controller
    {
        public IActionResult TestLogin()
        {
            HttpContext.Session.SetInt32("MaNV", 999);
            HttpContext.Session.SetInt32("MaVT", 1); // 1 = nhân viên phục vụ (thu ngân)
            HttpContext.Session.SetString("HoTenNV", "Nhan Vien Test");
            HttpContext.Session.SetString("TenVaiTro", "Nhan vien phuc vu");
            HttpContext.Session.SetString("LoaiTaiKhoan", "NhanVien");

            return RedirectToAction("Index");
        }
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