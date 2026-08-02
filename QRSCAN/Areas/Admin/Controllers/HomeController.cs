using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Phục vụ")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public class ThongKeMonAnViewModel
        {
            public int MaMon { get; set; }
            public string TenMon { get; set; }
            public string HinhAnh { get; set; }
            public string TenDM { get; set; }
            public int SoLuongBan { get; set; }
            public decimal DoanhThu { get; set; }
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult DanhSachMonDaBan(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = fromDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = toDate ?? DateTime.Now.Date;
            DateTime endOfDay = end.AddDays(1).AddTicks(-1);

            ViewBag.TuNgay = start.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = end.ToString("dd/MM/yyyy");

            // Lấy toàn bộ chi tiết đơn hàng đã hoàn thành trong khoảng thời gian
            var dsMonDaBan = _context.ChiTietDonHangs
                .Include(c => c.DonHang)
                .Include(c => c.MonAn)
                .ThenInclude(m => m.DanhMucMon)
                .Where(c => c.DonHang.TrangThai == "HoanThanh" && c.DonHang.ThoiGianTao >= start && c.DonHang.ThoiGianTao <= endOfDay)
                .ToList() // Kéo về RAM để GroupBy an toàn hơn
                .GroupBy(c => c.MonAn)
                .Select(g => new ThongKeMonAnViewModel
                {
                    MaMon = g.Key?.MaMon ?? 0,
                    TenMon = g.Key?.TenMon ?? "Món đã xóa",
                    HinhAnh = g.Key?.HinhAnh ?? "/images/default.png",
                    TenDM = g.Key?.DanhMucMon?.TenDM ?? "Khác",
                    SoLuongBan = g.Sum(c => c.SoLuong),
                    DoanhThu = g.Sum(c => c.ThanhTien)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .ToList();

            return View(dsMonDaBan);
        }

        [HttpGet]
        public IActionResult GetDashboardData(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = fromDate ?? DateTime.Now.AddMonths(-11).AddDays(-DateTime.Now.Day + 1).Date;
            DateTime end = toDate ?? DateTime.Now.Date;
            DateTime endOfDay = end.AddDays(1).AddTicks(-1);

            // SỬA: "HoanThanh"
            var hoaDons = _context.HoaDons
                .Include(h => h.DonHang)
                .Where(h => h.DonHang.TrangThai == "HoanThanh" && h.ThoiGianTao >= start && h.ThoiGianTao <= endOfDay)
                .ToList();

            return Json(new
            {
                success = true,
                tongDoanhThu = hoaDons.Sum(h => h.TongTien),
                tongDonHang = hoaDons.Count,
                bieuDoDoanhThu = hoaDons.GroupBy(h => new { h.ThoiGianTao.Year, h.ThoiGianTao.Month })
                                         .Select(g => new { Ngay = $"{g.Key.Month}/{g.Key.Year}", DoanhThu = g.Sum(h => h.TongTien) }),
                bieuDoMonAn = _context.ChiTietDonHangs
                    .Include(c => c.DonHang)
                    .Include(c => c.MonAn)
                    .Where(c => c.DonHang.TrangThai == "HoanThanh" && c.DonHang.ThoiGianTao >= start && c.DonHang.ThoiGianTao <= endOfDay)
                    .GroupBy(c => c.MonAn.TenMon)
                    .Select(g => new { TenMon = g.Key, SoLuong = g.Sum(c => c.SoLuong) })
                    .OrderByDescending(x => x.SoLuong).Take(5).ToList()
            });
        }
    }
}