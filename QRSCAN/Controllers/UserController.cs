using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(x => x.MaKH == maKH.Value);

            if (khachHang == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(khachHang);
        }

        public async Task<IActionResult> LichSuDonHang()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangs = await _context.DonHangs
                .Where(x => x.MaKH == maKH.Value)
                .OrderByDescending(x => x.ThoiGianDat)
                .ToListAsync();

            return View(donHangs);
        }

        public async Task<IActionResult> ChiTietDonHang(int maDonHang)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(x => x.MaDonHang == maDonHang && x.MaKH == maKH.Value);

            if (donHang == null)
            {
                return RedirectToAction("LichSuDonHang");
            }

            var chiTiet = await _context.ChiTietDonHangs
                .Include(x => x.MonAn)
                .Where(x => x.MaDonHang == maDonHang)
                .ToListAsync();

            ViewBag.DonHang = donHang;

            return View(chiTiet);
        }

        public async Task<IActionResult> TrangThaiDonHang()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangsDangXuLy = await _context.DonHangs
                .Where(x => x.MaKH == maKH.Value
                    && (x.TrangThai == "ChoXacNhan"
                        || x.TrangThai == "DangCheBien"
                        || x.TrangThai == "DaPhucVu"))
                .OrderByDescending(x => x.ThoiGianDat)
                .ToListAsync();

            return View(donHangsDangXuLy);
        }
    }
}