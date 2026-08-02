using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class KhachHangController : Controller
    {
        private readonly AppDbContext _context;

        public KhachHangController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var khachHangs = _context.KhachHangs.OrderByDescending(k => k.MaKH).ToList();
            return View(khachHangs);
        }

        [HttpGet]
        public IActionResult GetLichSuDonHang(int maKH)
        {
            // Liên kết Đơn hàng với Phiên gọi món để lọc theo MaKH
            var history = _context.DonHangs
                .Include(d => d.PhienGoiMon)
                .Where(d => d.PhienGoiMon != null && d.PhienGoiMon.MaKH == maKH)
                .OrderByDescending(d => d.ThoiGianTao)
                .Select(d => new {
                    maDH = d.MaDH,
                    thoiGian = d.ThoiGianTao.ToString("dd/MM/yyyy HH:mm"),
                    tongTien = d.TongTien,
                    soTienGiam = d.SoTienGiam,
                    thanhToan = d.TongTien - d.SoTienGiam,
                    trangThai = d.TrangThai
                })
                .ToList();

            return Json(new { success = true, data = history });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var kh = _context.KhachHangs.Find(id);
            if (kh == null) return NotFound();

            return Json(kh);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] KhachHang model)
        {
            var khCu = _context.KhachHangs.Find(model.MaKH);
            if (khCu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng này!" });
            }

            khCu.HoTen = model.HoTen;
            khCu.SDT = model.SDT;
            khCu.Email = model.Email;
            khCu.LoaiKhach = model.LoaiKhach;
            khCu.TrangThai = model.TrangThai;

            _context.SaveChanges();

            return Json(new { success = true, message = "Cập nhật khách hàng thành công!" });
        }
    }
}