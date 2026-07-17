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

            // 1. Kiểm tra tài khoản khách hàng
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh =>
                    kh.TenDangNhap == tenDangNhap
                    && kh.MatKhau == matKhau
                    && kh.TrangThai == "HoatDong");

            if (khachHang != null)
            {
                HttpContext.Session.SetInt32("MaKH", khachHang.MaKH);
                HttpContext.Session.SetString("HoTenKH", khachHang.HoTen);
                HttpContext.Session.SetString("LoaiTaiKhoan", "KhachHang");

                return RedirectToAction("Index", "Home");
            }

            // 2. Kiểm tra tài khoản nhân viên
            var nhanVien = await _context.NhanViens
                .Include(nv => nv.VaiTro)
                .FirstOrDefaultAsync(nv =>
                    nv.TenDangNhap == tenDangNhap
                    && nv.MatKhau == matKhau
                    && nv.TrangThai == "HoatDong");

            if (nhanVien != null)
            {
                HttpContext.Session.SetInt32("MaNV", nhanVien.MaNV);
                HttpContext.Session.SetInt32("MaVT", nhanVien.MaVT);
                HttpContext.Session.SetString("HoTenNV", nhanVien.HoTen);
                HttpContext.Session.SetString("TenVaiTro", nhanVien.VaiTro?.TenVT ?? "");
                HttpContext.Session.SetString("LoaiTaiKhoan", "NhanVien");

                // Bếp
                if (nhanVien.MaVT == 2 || nhanVien.VaiTro?.TenVT == "Bep")
                {
                    return RedirectToAction("Index", "Chef");
                }

                // Thu ngân
                if (nhanVien.MaVT == 4 || nhanVien.VaiTro?.TenVT == "ThuNgan")
                {
                    return RedirectToAction("DanhSach", "ThanhToan");
                }

                // Admin hoặc nhân viên phục vụ
                return RedirectToAction("Index", "NhanVien");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            string hoTen,
            string sdt,
            string email,
            string tenDangNhap,
            string matKhau)
        {
            if (string.IsNullOrWhiteSpace(hoTen)
                || string.IsNullOrWhiteSpace(tenDangNhap)
                || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ họ tên, tên đăng nhập và mật khẩu.";
                return View();
            }

            var trungTenDangNhap = await _context.KhachHangs
                .AnyAsync(kh => kh.TenDangNhap == tenDangNhap);

            var trungNhanVien = await _context.NhanViens
                .AnyAsync(nv => nv.TenDangNhap == tenDangNhap);

            if (trungTenDangNhap || trungNhanVien)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            if (!string.IsNullOrWhiteSpace(sdt))
            {
                var trungSDT = await _context.KhachHangs
                    .AnyAsync(kh => kh.SDT == sdt);

                if (trungSDT)
                {
                    ViewBag.Error = "Số điện thoại đã được sử dụng.";
                    return View();
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var trungEmail = await _context.KhachHangs
                    .AnyAsync(kh => kh.Email == email);

                if (trungEmail)
                {
                    ViewBag.Error = "Email đã được sử dụng.";
                    return View();
                }
            }

            var khachHang = new KhachHang
            {
                HoTen = hoTen,
                SDT = sdt,
                Email = email,
                LoaiKhach = "Thuong",
                DiemTichLuy = 0,
                TrangThai = "HoatDong",
                TenDangNhap = tenDangNhap,
                MatKhau = matKhau
            };

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("MaKH", khachHang.MaKH);
            HttpContext.Session.SetString("HoTenKH", khachHang.HoTen);
            HttpContext.Session.SetString("LoaiTaiKhoan", "KhachHang");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}