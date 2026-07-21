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

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult GetDashboardData(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = fromDate ?? DateTime.Now.AddMonths(-11).AddDays(-DateTime.Now.Day + 1).Date;
            DateTime end = toDate ?? DateTime.Now.Date;
            DateTime endOfDay = end.AddDays(1).AddTicks(-1);

            var hoaDons = _context.DonHangs
                .Where(d => d.TrangThai == "Hoàn thành" && d.ThoiGianTao >= start && d.ThoiGianTao <= endOfDay)
                .ToList();

            return Json(new
            {
                success = true,
                tongDoanhThu = hoaDons.Sum(d => d.TongTien),
                tongDonHang = hoaDons.Count,
                bieuDoDoanhThu = hoaDons.GroupBy(d => new { d.ThoiGianTao.Year, d.ThoiGianTao.Month })
                                         .Select(g => new { Ngay = $"{g.Key.Month}/{g.Key.Year}", DoanhThu = g.Sum(d => d.TongTien) }),
                bieuDoMonAn = _context.ChiTietDonHangs
                    .Include(c => c.MonAn)
                    .Where(c => c.DonHang.TrangThai == "Hoàn thành" && c.DonHang.ThoiGianTao >= start && c.DonHang.ThoiGianTao <= endOfDay)
                    .GroupBy(c => c.MonAn.TenMon)
                    .Select(g => new { TenMon = g.Key, SoLuong = g.Sum(c => c.SoLuong) })
                    .OrderByDescending(x => x.SoLuong).Take(5).ToList()
            });
        }
    }
}