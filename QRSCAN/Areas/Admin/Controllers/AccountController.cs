using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using System.Security.Claims;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
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
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            var nhanVien = _context.NhanViens
                .Include(n => n.VaiTro)
                .FirstOrDefault(x => x.TenDangNhap == tenDangNhap && x.MatKhau == matKhau);

            if (nhanVien != null)
            {
                if (nhanVien.VaiTro == null || nhanVien.VaiTro.TenVT != "Admin")
                {
                    ViewBag.Error = "Tài khoản của bạn không có quyền truy cập trang quản trị!";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, nhanVien.HoTen),
                    new Claim(ClaimTypes.NameIdentifier, nhanVien.MaNV.ToString()),
                    new Claim(ClaimTypes.Role, nhanVien.VaiTro.TenVT)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
    }
}
