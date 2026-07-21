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
            // Dùng MaVT thay vì MaVaiTro
            var nhanViens = _context.NhanViens.Include(n => n.VaiTro).ToList();
            ViewBag.VaiTros = _context.VaiTros.ToList();

            return View(nhanViens);
        }

        [HttpPost]
        public IActionResult Create([FromBody] NhanVien model)
        {
            if (string.IsNullOrEmpty(model.TenDangNhap)) return Json(new { success = false });
            _context.NhanViens.Add(model);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var nv = _context.NhanViens.Find(id); // MaNV
            if (nv == null) return NotFound();
            return Json(nv);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] NhanVien model)
        {
            var nvCu = _context.NhanViens.Find(model.MaNV);
            if (nvCu == null) return Json(new { success = false });

            nvCu.MaVT = model.MaVT; // Khớp với Model
            nvCu.HoTen = model.HoTen;
            nvCu.SDT = model.SDT;
            nvCu.Email = model.Email;
            nvCu.TrangThai = model.TrangThai;

            if (!string.IsNullOrEmpty(model.MatKhau)) nvCu.MatKhau = model.MatKhau;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}