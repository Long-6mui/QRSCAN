using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly AppDbContext _context;

        public ThanhToanController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsNhanVienDangNhap()
        {
            return HttpContext.Session.GetInt32("MaNV") != null;
        }

        public async Task<IActionResult> DanhSach()
        {
            if (!IsNhanVienDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var dsHoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d!.PhienGoiMon)
                        .ThenInclude(p => p!.BanAn)
                .Include(h => h.PhuongThucThanhToan)
                .Where(h => h.PhuongThucThanhToan != null
                    && h.PhuongThucThanhToan.TenPT == "Tien mat"
                    && h.TrangThai == "ChuaThanhToan")
                .OrderBy(h => h.ThoiGianTao)
                .ToListAsync();

            ViewBag.ActiveTab = "thu";

            return View(dsHoaDon);
        }

        public async Task<IActionResult> ThuTien(int maDH)
        {
            if (!IsNhanVienDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .FirstOrDefaultAsync(d => d.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanDaThanhToan(int maDH)
        {
            if (!IsNhanVienDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                .Where(h => h.MaDH == maDH)
                .OrderByDescending(h => h.MaHD)
                .FirstOrDefaultAsync();

            if (hoaDon == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("DanhSach");
            }

            hoaDon.TrangThai = "DaThanhToan";
            hoaDon.ThoiGianTao = DateTime.Now;
            hoaDon.MaNV = HttpContext.Session.GetInt32("MaNV");

            if (hoaDon.DonHang != null)
            {
                hoaDon.DonHang.TrangThai = "DaThanhToan";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("HoaDon", new { maDH });
        }

        public async Task<IActionResult> DanhSachHoaDon()
        {
            if (!IsNhanVienDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var dsHoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d!.PhienGoiMon)
                        .ThenInclude(p => p!.BanAn)
                .Include(h => h.PhuongThucThanhToan)
                .Include(h => h.NhanVien)
                .Where(h => h.TrangThai == "DaThanhToan")
                .OrderByDescending(h => h.ThoiGianTao)
                .ToListAsync();

            ViewBag.ActiveTab = "hoadon";

            return View(dsHoaDon);
        }

        public async Task<IActionResult> HoaDon(int maDH)
        {
            if (!IsNhanVienDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d!.PhienGoiMon)
                        .ThenInclude(p => p!.BanAn)
                .Include(h => h.DonHang)
                    .ThenInclude(d => d!.ChiTietDonHangs)
                        .ThenInclude(ct => ct.MonAn)
                .Include(h => h.PhuongThucThanhToan)
                .Include(h => h.NhanVien)
                .FirstOrDefaultAsync(h => h.MaDH == maDH);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
    }
}