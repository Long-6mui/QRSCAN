using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

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
        public IActionResult Index(DateTime? fromDate, DateTime? toDate, string customerName, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.DonHangs
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.ThoiGianDat >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                var nextDay = toDate.Value.Date.AddDays(1);
                query = query.Where(d => d.ThoiGianDat < nextDay);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(d => d.TongThanhToan >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(d => d.TongThanhToan <= maxPrice.Value);
            }

            var donHangs = query.OrderByDescending(d => d.ThoiGianDat).ToList();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CustomerName = customerName;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(donHangs);
        }

        [HttpGet]
        public IActionResult GetChiTiet(int id)
        {
            var donHang = _context.DonHangs.Find(id);
            if (donHang == null) return NotFound();

            var chiTiets = _context.ChiTietDonHangs
                .Include(c => c.MonAn) 
                .Where(c => c.MaDonHang == id)
                .Select(c => new
                {
                    tenMon = c.MonAn != null ? c.MonAn.TenMon : "Món đã xóa",
                    hinhAnh = c.MonAn != null ? c.MonAn.HinhAnh : "/images/default-food.jpg",
                    soLuong = c.SoLuong,
                    donGia = c.DonGia,
                    thanhTien = c.ThanhTien
                })
                .ToList();

            return Json(new
            {
                success = true,
                maDonHang = donHang.MaDonHang,
                thoiGian = donHang.ThoiGianDat.ToString("dd/MM/yyyy HH:mm"),
                trangThai = donHang.TrangThai,
                chiTiets = chiTiets,
                tongTien = donHang.TongTien,
                tienGiam = donHang.TienGiam,
                tongThanhToan = donHang.TongThanhToan
            });
        }
    }
}