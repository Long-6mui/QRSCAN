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

        // Danh sách đơn hàng thanh toán tiền mặt đang chờ thu ngân xử lý (dạng lưới số bàn)
        public async Task<IActionResult> DanhSach()
        {
            if (!IsNhanVienDangNhap()) return RedirectToAction("Login", "Account");

            var dsDon = await _context.ThanhToans
                .Where(t => t.PhuongThuc == "TienMat" && t.TrangThai == "ChoThanhToan")
                .OrderBy(t => t.NgayThanhToan)
                .ToListAsync();

            var maDonHangs = dsDon.Select(t => t.MaDonHang).Distinct().ToList();
            var donHangs = await _context.DonHangs
                .Where(d => maDonHangs.Contains(d.MaDonHang))
                .ToDictionaryAsync(d => d.MaDonHang);

            foreach (var t in dsDon)
            {
                if (donHangs.TryGetValue(t.MaDonHang, out var dh))
                    t.DonHang = dh;
            }

            ViewBag.ActiveTab = "thu";

            return View(dsDon);
        }

        // GET: hiển thị chi tiết đơn trước khi thu ngân bấm xác nhận
        public async Task<IActionResult> ThuTien(int maDonHang)
        {
            if (!IsNhanVienDangNhap()) return RedirectToAction("Login", "Account");

            var donHang = await _context.DonHangs
                .Include(x => x.ChiTietDonHangs!)
                    .ThenInclude(ct => ct.MonAn)
                .FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // Xác nhận đã thu tiền mặt (thu ngân chỉ cần bấm 1 nút)
        [HttpPost]
        public async Task<IActionResult> XacNhanDaThanhToan(int maDonHang)
        {
            if (!IsNhanVienDangNhap()) return RedirectToAction("Login", "Account");

            var thanhToan = await _context.ThanhToans
                .Where(t => t.MaDonHang == maDonHang)
                .OrderByDescending(t => t.MaThanhToan)
                .FirstOrDefaultAsync();

            if (thanhToan == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DanhSach");
            }

            thanhToan.TrangThai = "DaThanhToan";
            thanhToan.NgayThanhToan = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("HoaDon", new { maDonHang });
        }

        // Danh sách hóa đơn đã thanh toán (dạng lưới số bàn)
        public async Task<IActionResult> DanhSachHoaDon()
        {
            if (!IsNhanVienDangNhap()) return RedirectToAction("Login", "Account");

            var dsHoaDon = await _context.ThanhToans
                .Where(t => t.TrangThai == "DaThanhToan")
                .OrderByDescending(t => t.NgayThanhToan)
                .ToListAsync();

            var maDonHangs = dsHoaDon.Select(t => t.MaDonHang).Distinct().ToList();
            var donHangs = await _context.DonHangs
                .Where(d => maDonHangs.Contains(d.MaDonHang))
                .ToDictionaryAsync(d => d.MaDonHang);

            foreach (var t in dsHoaDon)
            {
                if (donHangs.TryGetValue(t.MaDonHang, out var dh))
                    t.DonHang = dh;
            }

            ViewBag.ActiveTab = "hoadon";

            return View(dsHoaDon);
        }

        // Xem chi tiết 1 hóa đơn
        public async Task<IActionResult> HoaDon(int maDonHang)
        {
            if (!IsNhanVienDangNhap()) return RedirectToAction("Login", "Account");

            var donHang = await _context.DonHangs
                .Include(x => x.ChiTietDonHangs!)
                    .ThenInclude(ct => ct.MonAn)
                .FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);

            if (donHang == null) return NotFound();

            var thanhToan = await _context.ThanhToans
                .Where(t => t.MaDonHang == maDonHang)
                .OrderByDescending(t => t.MaThanhToan)
                .FirstOrDefaultAsync();

            ViewBag.ThanhToan = thanhToan;
            return View(donHang);
        }
    }
}