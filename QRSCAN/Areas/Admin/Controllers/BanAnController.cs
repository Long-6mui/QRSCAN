using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class BanAnController : Controller
    {
        private readonly AppDbContext _context;

        public BanAnController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var banAns = _context.BanAns.ToList();
            return View(banAns);
        }

        [HttpPost]
        public IActionResult Create([FromBody] BanAn model)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenBan)) errors.Add("TenBan", "Tên bàn không được trống.");
            else if (_context.BanAns.Any(b => b.TenBan.ToLower() == model.TenBan.ToLower())) errors.Add("TenBan", "Tên bàn đã tồn tại.");

            if (model.SoChoNgoi <= 0) errors.Add("SoChoNgoi", "Số chỗ ngồi phải > 0.");

            if (errors.Any()) return Json(new { success = false, errors = errors });

            model.NoiDungQR = "https://qrscan.com/order?table=" + model.TenBan.Replace(" ", "");

            _context.BanAns.Add(model);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var banAn = _context.BanAns.Find(id);
            if (banAn == null) return NotFound();
            return Json(banAn);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] BanAn model)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenBan)) errors.Add("TenBan", "Tên bàn không được trống.");
            else if (_context.BanAns.Any(b => b.TenBan.ToLower() == model.TenBan.ToLower() && b.MaBan != model.MaBan)) errors.Add("TenBan", "Tên bàn đã tồn tại.");

            if (model.SoChoNgoi <= 0) errors.Add("SoChoNgoi", "Số chỗ ngồi phải > 0.");

            if (errors.Any()) return Json(new { success = false, errors = errors });

            var banAnCu = _context.BanAns.Find(model.MaBan);
            if (banAnCu == null) return Json(new { success = false, message = "Không tìm thấy bàn!" });

            banAnCu.TenBan = model.TenBan;
            banAnCu.SoChoNgoi = model.SoChoNgoi;
            banAnCu.TrangThai = model.TrangThai;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}