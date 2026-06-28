using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(KhachHang khachHang)
        {
            if (!ModelState.IsValid)
            {
                return View(khachHang);
            }

            var tenDangNhapTonTai = await _context.KhachHangs
                .AnyAsync(k => k.TenDangNhap == khachHang.TenDangNhap);

            if (tenDangNhapTonTai)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                return View(khachHang);
            }

            var sdtTonTai = await _context.KhachHangs
                .AnyAsync(k => k.SDT == khachHang.SDT);

            if (sdtTonTai)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được sử dụng");
                return View(khachHang);
            }

            khachHang.LoaiKhach = "Thuong";
            khachHang.DiemTichLuy = 0;
            khachHang.TrangThai = "HoatDong";

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu";
                return View();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k =>
                    k.TenDangNhap == tenDangNhap &&
                    k.MatKhau == matKhau &&
                    k.TrangThai == "HoatDong");

            if (khachHang == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }

            HttpContext.Session.SetInt32("MaKH", khachHang.MaKH);
            HttpContext.Session.SetString("HoTenKH", khachHang.HoTen);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}