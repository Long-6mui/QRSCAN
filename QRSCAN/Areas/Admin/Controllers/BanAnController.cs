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
            if (string.IsNullOrEmpty(model.TenBan))
            {
                return Json(new { success = false, message = "Tên bàn không được để trống!" });
            }

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
            var banAnCu = _context.BanAns.Find(model.MaBan);
            if (banAnCu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bàn ăn!" });
            }

            banAnCu.TenBan = model.TenBan;
            banAnCu.SoChoNgoi = model.SoChoNgoi;
            banAnCu.TrangThai = model.TrangThai;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}