using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDashboardData(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = fromDate ?? DateTime.Now.AddYears(-1).AddMonths(-11).AddDays(-DateTime.Now.Day + 1).Date;
            DateTime end = toDate ?? DateTime.Now.Date;
            DateTime endOfDay = end.AddDays(1).AddTicks(-1);

            // 3. Lọc Đơn hàng ĐÃ HOÀN THÀNH trong khoảng thời gian
            var donHangs = _context.DonHangs
                .Where(d => d.TrangThai == "Đã thanh toán" && d.ThoiGianDat >= start && d.ThoiGianDat <= endOfDay)
                .ToList();

            // Tổng quan (Thẻ Card)
            var tongDoanhThu = donHangs.Sum(d => d.TongThanhToan);
            var tongDonHang = donHangs.Count;

            // 4. Dữ liệu Biểu đồ Doanh thu theo ngày
            //var doanhThuTheoNgay = donHangs
            //    .GroupBy(d => d.ThoiGianDat.Date)
            //    .Select(g => new
            //    {
            //        Ngay = g.Key.ToString("dd/MM"),
            //        DoanhThu = g.Sum(d => d.TongThanhToan)
            //    })
            //    .OrderBy(x => x.Ngay)
            //    .ToList();
            // 4. Dữ liệu Biểu đồ Doanh thu theo tháng
            var doanhThuTheoThang = donHangs
                .GroupBy(d => new { d.ThoiGianDat.Year, d.ThoiGianDat.Month })
                .Select(g => new
                {
                    Ngay = $"{g.Key.Month}/{g.Key.Year}",
                    SortKey = new DateTime(g.Key.Year, g.Key.Month, 1),
                    DoanhThu = g.Sum(d => d.TongThanhToan)
                })
                .OrderBy(x => x.SortKey)
                .ToList();


            // 5. Dữ liệu Biểu đồ Top 5 Món bán chạy nhất
            var topMonAn = _context.ChiTietDonHangs
                .Include(c => c.DonHang)
                .Include(c => c.MonAn)
                .Where(c => c.DonHang.TrangThai == "Đã thanh toán" && c.DonHang.ThoiGianDat >= start && c.DonHang.ThoiGianDat <= endOfDay)
                .GroupBy(c => c.MonAn.TenMon)
                .Select(g => new
                {
                    TenMon = g.Key,
                    SoLuong = g.Sum(c => c.SoLuong)
                })
                .OrderByDescending(x => x.SoLuong)  
                .Take(5)
                .ToList();

            return Json(new
            {
                success = true,
                tongDoanhThu = tongDoanhThu,
                tongDonHang = tongDonHang,
                bieuDoDoanhThu = doanhThuTheoThang,
                bieuDoMonAn = topMonAn
            });
        }
    }
}
