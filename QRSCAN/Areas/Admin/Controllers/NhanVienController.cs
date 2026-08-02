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
            var errors = new Dictionary<string, string>();

            // VALIDATION: Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(model.TenDangNhap))
                errors.Add("TenDangNhap", "Tên đăng nhập không được để trống.");
            else if (_context.NhanViens.Any(n => n.TenDangNhap == model.TenDangNhap))
                errors.Add("TenDangNhap", "Tên đăng nhập này đã tồn tại!");

            if (string.IsNullOrWhiteSpace(model.MatKhau))
                errors.Add("MatKhau", "Mật khẩu không được để trống.");

            if (string.IsNullOrWhiteSpace(model.HoTen))
                errors.Add("HoTen", "Họ và tên không được để trống.");

            // VALIDATION: Kiểm tra định dạng Email (nếu có nhập)
            if (!string.IsNullOrEmpty(model.Email) && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(model.Email))
                errors.Add("Email", "Email không đúng định dạng!");

            // Nếu có lỗi, trả về danh sách lỗi
            if (errors.Any())
                return Json(new { success = false, errors = errors });

            _context.NhanViens.Add(model);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var nv = _context.NhanViens.Find(id); // Dùng đúng DbSet NhanVien
            if (nv == null) return NotFound();
            return Json(nv);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] NhanVien model)
        {
            var errors = new Dictionary<string, string>();
            var nvCu = _context.NhanViens.Find(model.MaNV); // Dùng đúng DbSet NhanVien

            if (nvCu == null)
                return Json(new { success = false, message = "Không tìm thấy nhân viên!" });

            // VALIDATION: Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(model.HoTen))
                errors.Add("HoTen", "Họ và tên không được để trống.");

            // VALIDATION: Kiểm tra định dạng Email (nếu có nhập)
            if (!string.IsNullOrEmpty(model.Email) && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(model.Email))
                errors.Add("Email", "Email không đúng định dạng!");

            if (errors.Any())
                return Json(new { success = false, errors = errors });

            nvCu.MaVT = model.MaVT;
            nvCu.HoTen = model.HoTen;
            nvCu.SDT = model.SDT;
            nvCu.Email = model.Email;
            nvCu.TrangThai = model.TrangThai;

            // Chỉ cập nhật mật khẩu nếu người dùng có nhập vào ô
            if (!string.IsNullOrEmpty(model.MatKhau)) nvCu.MatKhau = model.MatKhau;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}