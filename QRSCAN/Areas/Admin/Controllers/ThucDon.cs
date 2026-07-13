using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ThucDonController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ThucDonController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            ViewBag.MonAns = _context.MonAns.Include(m => m.DanhMucMon).OrderByDescending(m => m.MaMon).ToList();
            ViewBag.DanhMucs = _context.DanhMucMons.OrderByDescending(d => d.MaDanhMuc).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMonAn([FromForm] MonAn model, IFormFile? FileHinhAnh)
        {
            if (string.IsNullOrEmpty(model.TenMon) || model.DonGia <= 0 || model.MaDanhMuc <= 0)
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ Tên món, Danh mục và Giá!" });

            if (FileHinhAnh != null && FileHinhAnh.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileHinhAnh.FileName);
                string uploadPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await FileHinhAnh.CopyToAsync(stream);
                }
                model.HinhAnh = "/images/" + fileName;
            }
            else
            {
                model.HinhAnh = "/images/default-food.jpg";
            }

            _context.MonAns.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetMonAnById(int id)
        {
            var monAn = _context.MonAns.Find(id);
            if (monAn == null) return NotFound();
            return Json(monAn);
        }

        [HttpPost]
        public async Task<IActionResult> EditMonAn([FromForm] MonAn model, IFormFile? FileHinhAnh)
        {
            var monAnCu = await _context.MonAns.FindAsync(model.MaMon);
            if (monAnCu == null) return Json(new { success = false, message = "Không tìm thấy món ăn!" });

            monAnCu.TenMon = model.TenMon;
            monAnCu.MoTa = model.MoTa;
            monAnCu.DonGia = model.DonGia;
            monAnCu.TrangThai = model.TrangThai; 
            monAnCu.MaDanhMuc = model.MaDanhMuc;

            if (FileHinhAnh != null && FileHinhAnh.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileHinhAnh.FileName);
                string uploadPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await FileHinhAnh.CopyToAsync(stream);
                }
                monAnCu.HinhAnh = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CreateDanhMuc([FromBody] DanhMucMon model)
        {
            if (string.IsNullOrEmpty(model.TenDanhMuc))
                return Json(new { success = false, message = "Tên danh mục không được để trống!" });

            _context.DanhMucMons.Add(model);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetDanhMucById(int id)
        {
            var dm = _context.DanhMucMons.Find(id);
            if (dm == null) return NotFound();
            return Json(dm);
        }

        [HttpPost]
        public IActionResult EditDanhMuc([FromBody] DanhMucMon model)
        {
            var dmCu = _context.DanhMucMons.Find(model.MaDanhMuc);
            if (dmCu == null) return Json(new { success = false, message = "Không tìm thấy danh mục!" });

            dmCu.TenDanhMuc = model.TenDanhMuc;
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}