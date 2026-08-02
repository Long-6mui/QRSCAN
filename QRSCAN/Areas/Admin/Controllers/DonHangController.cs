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
        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int? searchId, int page = 1)
        {
            // Số lượng đơn hàng hiển thị trên 1 trang
            int pageSize = 15;

            var query = _context.DonHangs.Include(d => d.PhienGoiMon).ThenInclude(p => p.KhachHang).AsQueryable();

            if (searchId.HasValue)
            {
                query = query.Where(d => d.MaDH == searchId.Value);
            }
            else
            {
                if (fromDate.HasValue)
                    query = query.Where(d => d.ThoiGianTao.Date >= fromDate.Value.Date);
                if (toDate.HasValue)
                    query = query.Where(d => d.ThoiGianTao.Date <= toDate.Value.Date);
            }

            // 1. Tính tổng số dòng và tổng số trang
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // 2. Cắt dữ liệu (Phân trang)
            var donHangs = query.OrderByDescending(d => d.ThoiGianTao)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            // 3. Truyền dữ liệu ra View bằng ViewBag để vẽ thanh phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SearchId = searchId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

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
                .Where(c => c.MaDH == id) // Dùng MaDH
                .Select(c => new {
                    tenMon = c.MonAn != null ? c.MonAn.TenMon : "Món đã xóa",
                    soLuong = c.SoLuong,
                    donGia = c.DonGia,
                    thanhTien = c.ThanhTien
                }).ToList();

            return Json(new
            {
                success = true,
                maDH = donHang.MaDH,
                thoiGian = donHang.ThoiGianTao.ToString("dd/MM/yyyy HH:mm"),
                trangThai = donHang.TrangThai,
                tongTien = donHang.TongTien,
                tienGiam = donHang.SoTienGiam,
                tongThanhToan = donHang.TongTien - donHang.SoTienGiam,
                chiTiets = chiTiets // <--- ĐÃ THÊM DÒNG NÀY ĐỂ TRẢ DỮ LIỆU VỀ VIEW
            });
        }
    }
}