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

        private int? LayMaKH()
        {
            return HttpContext.Session.GetInt32("MaKH");
        }

        public async Task<IActionResult> Profile()
        {
            var maKH = LayMaKH();

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
            var maKH = LayMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangs = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .Where(d => d.PhienGoiMon != null
                    && d.PhienGoiMon.MaKH == maKH.Value)
                .OrderByDescending(d => d.ThoiGianTao)
                .ToListAsync();

            return View(donHangs);
        }

        public async Task<IActionResult> ChiTietDonHang(int maDH)
        {
            var maKH = LayMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.Voucher)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .FirstOrDefaultAsync(d => d.MaDH == maDH
                    && d.PhienGoiMon != null
                    && d.PhienGoiMon.MaKH == maKH.Value);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        public async Task<IActionResult> TrangThaiDonHang()
        {
            var maKH = LayMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangsDangXuLy = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .Where(d => d.PhienGoiMon != null
                    && d.PhienGoiMon.MaKH == maKH.Value
                    && (d.TrangThai == "ChoXacNhan"
                        || d.TrangThai == "DangCheBien"
                        || d.TrangThai == "DaPhucVu"))
                .OrderByDescending(d => d.ThoiGianTao)
                .ToListAsync();

            return View(donHangsDangXuLy);
        }
    }
}