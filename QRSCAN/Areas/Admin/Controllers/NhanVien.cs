using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class NhanVienController : Controller
    {
        private readonly AppDbContext _context;

        public NhanVienController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var nhanViens = _context.NhanViens.Include(n => n.VaiTro).ToList();

            ViewBag.VaiTros = _context.VaiTros.ToList();

            return View(nhanViens);
        }

        [HttpPost]
        public IActionResult Create([FromBody] NhanVien model)
        {
            if (string.IsNullOrEmpty(model.TenDangNhap) || string.IsNullOrEmpty(model.MatKhau))
                return Json(new { success = false, message = "Tên đăng nhập và mật khẩu không được để trống!" });

            if (_context.NhanViens.Any(n => n.TenDangNhap == model.TenDangNhap))
                return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại!" });

            _context.NhanViens.Add(model);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var nv = _context.NhanViens.Find(id);
            if (nv == null) return NotFound();
            return Json(nv);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] NhanVien model)
        {
            var nvCu = _context.NhanViens.Find(model.MaNV);
            if (nvCu == null) return Json(new { success = false, message = "Không tìm thấy nhân viên!" });

            if (nvCu.TenDangNhap != model.TenDangNhap && _context.NhanViens.Any(n => n.TenDangNhap == model.TenDangNhap))
            {
                return Json(new { success = false, message = "Tên đăng nhập này đã thuộc về người khác!" });
            }

            nvCu.MaVT = model.MaVT;
            nvCu.HoTen = model.HoTen;
            nvCu.SDT = model.SDT;
            nvCu.Email = model.Email;
            nvCu.TenDangNhap = model.TenDangNhap;

            if (!string.IsNullOrEmpty(model.MatKhau))
            {
                nvCu.MatKhau = model.MatKhau;
            }

            nvCu.TrangThai = model.TrangThai;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}