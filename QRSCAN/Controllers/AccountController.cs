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
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            tenDangNhap = tenDangNhap.Trim();

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(x =>
                    x.TenDangNhap == tenDangNhap &&
                    x.MatKhau == matKhau);

            if (khachHang != null)
            {
                HttpContext.Session.SetInt32("MaKH", khachHang.MaKH);
                HttpContext.Session.SetString("HoTenKH", khachHang.HoTen);
                HttpContext.Session.SetString("LoaiTaiKhoan", "KhachHang");

                return RedirectToAction("Index", "Home");
            }

            var nhanVien = await _context.NhanViens
                .Include(x => x.VaiTro)
                .FirstOrDefaultAsync(x =>
                    x.TenDangNhap == tenDangNhap &&
                    x.MatKhau == matKhau &&
                    x.TrangThai == "HoatDong");

            if (nhanVien != null)
            {
                HttpContext.Session.SetInt32("MaNV", nhanVien.MaNV);
                HttpContext.Session.SetInt32("MaVT", nhanVien.MaVT);
                HttpContext.Session.SetString("HoTenNV", nhanVien.HoTen);
                HttpContext.Session.SetString("TenVaiTro", nhanVien.VaiTro?.TenVT ?? "");
                HttpContext.Session.SetString("LoaiTaiKhoan", "NhanVien");

                return RedirectToAction("Index", "NhanVien");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}