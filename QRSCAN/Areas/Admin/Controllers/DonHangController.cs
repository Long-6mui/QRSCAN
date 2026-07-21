using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Phục vụ")]
    public class DonHangController : Controller
    {
        private readonly AppDbContext _context;

        public DonHangController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.DonHangs.Include(d => d.PhienGoiMon).ThenInclude(p => p.KhachHang).AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(d => d.ThoiGianTao.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(d => d.ThoiGianTao.Date <= toDate.Value.Date);

            var donHangs = query.OrderByDescending(d => d.ThoiGianTao).ToList();
            return View(donHangs);
        }

        [HttpGet]
        public IActionResult GetChiTiet(int id)
        {
            // id ở đây là MaDH
            var donHang = _context.DonHangs.Find(id);
            if (donHang == null) return NotFound();

            var chiTiets = _context.ChiTietDonHangs
                .Include(c => c.MonAn)
                .Where(c => c.MaDH == id)
                .Select(c => new {
                    tenMon = c.MonAn != null ? c.MonAn.TenMon : "Món đã xóa",
                    soLuong = c.SoLuong,
                    donGia = c.DonGia,
                    thanhTien = c.ThanhTien
                }).ToList();

            return Json(new
            {
                success = true,
                thoiGian = donHang.ThoiGianTao.ToString("dd/MM/yyyy HH:mm"),
                trangThai = donHang.TrangThai,
                tongTien = donHang.TongTien,
                tienGiam = donHang.SoTienGiam,
                tongThanhToan = donHang.TongTien - donHang.SoTienGiam,
                chiTiets = chiTiets
            });
        }
    }
}