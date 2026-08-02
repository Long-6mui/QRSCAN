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
            ViewBag.DanhMucs = _context.DanhMucMons.OrderByDescending(d => d.MaDM).ToList();
            return View();
        }

        // ==========================================
        // 1. CÁC HÀM XỬ LÝ MÓN ĂN
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateMonAn([FromForm] MonAn model, IFormFile? FileHinhAnh)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenMon)) errors.Add("TenMon", "Tên món không được để trống!");
            if (model.DonGia <= 0) errors.Add("DonGia", "Đơn giá phải lớn hơn 0đ!");
            if (model.MaDM <= 0) errors.Add("MaDM", "Vui lòng chọn Danh mục!");

            if (errors.Any()) return Json(new { success = false, errors = errors });

            try
            {
                // 2. Xử lý lưu ảnh cục bộ (Local Storage)
                if (FileHinhAnh != null && FileHinhAnh.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileHinhAnh.FileName);
                    string uploadPath = Path.Combine(_env.WebRootPath, "images");

                    // Tự động tạo thư mục nếu chưa có
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileHinhAnh.CopyToAsync(stream);
                    }

                    // Chỉ lưu chuỗi URL vào database
                    model.HinhAnh = "/images/" + fileName;
                }
                else
                {
                    // Nếu không có ảnh tải lên, dùng ảnh mặc định
                    model.HinhAnh = "/images/default-food.jpg";
                }

                _context.MonAns.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi CSDL: " + ex.Message });
            }
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
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenMon)) errors.Add("TenMon", "Tên món không được để trống!");
            if (model.DonGia <= 0) errors.Add("DonGia", "Đơn giá phải lớn hơn 0đ!");
            if (model.MaDM <= 0) errors.Add("MaDM", "Vui lòng chọn Danh mục!");

            if (errors.Any()) return Json(new { success = false, errors = errors });

            try
            {
                var monAnCu = await _context.MonAns.FindAsync(model.MaMon);
                if (monAnCu == null) return Json(new { success = false, message = "Không tìm thấy món ăn!" });

                monAnCu.TenMon = model.TenMon;
                monAnCu.MoTa = model.MoTa;
                monAnCu.DonGia = model.DonGia;
                monAnCu.TrangThai = model.TrangThai;
                monAnCu.MaDM = model.MaDM;

                if (FileHinhAnh != null && FileHinhAnh.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileHinhAnh.FileName);
                    string uploadPath = Path.Combine(_env.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await FileHinhAnh.CopyToAsync(stream);
                    }
                    monAnCu.HinhAnh = "/images/menu/" + fileName;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // ==========================================
        // 2. CÁC HÀM XỬ LÝ DANH MỤC
        // ==========================================
        [HttpPost]
        public IActionResult CreateDanhMuc([FromBody] DanhMucMon model)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenDM)) errors.Add("TenDM", "Tên danh mục không được trống!");

            if (errors.Any()) return Json(new { success = false, errors = errors });

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
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenDM)) errors.Add("TenDM", "Tên danh mục không được trống!");

            if (errors.Any()) return Json(new { success = false, errors = errors });

            var dmCu = _context.DanhMucMons.Find(model.MaDM);
            if (dmCu == null) return Json(new { success = false, message = "Không tìm thấy danh mục!" });

            dmCu.TenDM = model.TenDM;
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteDanhMuc([FromBody] int id)
        {
            var dm = _context.DanhMucMons.Find(id);
            if (dm == null) return Json(new { success = false, message = "Không tìm thấy danh mục!" });

            // Kiểm tra xem danh mục này có món ăn nào không
            if (_context.MonAns.Any(m => m.MaDM == id))
                return Json(new { success = false, message = "Không thể xóa danh mục đang chứa món ăn!" });

            _context.DanhMucMons.Remove(dm);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}