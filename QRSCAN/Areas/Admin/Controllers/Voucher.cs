using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VoucherController : Controller
    {
        private readonly AppDbContext _context;

        public VoucherController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vouchers = _context.Vouchers.ToList();
            return View(vouchers);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Voucher model)
        {
            if (string.IsNullOrEmpty(model.MaCode))
                return Json(new { success = false, message = "Mã Code không được để trống!" });

            if (model.NgayBatDau >= model.NgayKetThuc)
                return Json(new { success = false, message = "Ngày kết thúc phải lớn hơn ngày bắt đầu!" });

            _context.Vouchers.Add(model);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var voucher = _context.Vouchers.Find(id);
            if (voucher == null) return NotFound();
            return Json(voucher);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] Voucher model)
        {
            var oldVoucher = _context.Vouchers.Find(model.MaVoucher);
            if (oldVoucher == null) return Json(new { success = false, message = "Lỗi dữ liệu!" });

            oldVoucher.MaCode = model.MaCode;
            oldVoucher.TenVoucher = model.TenVoucher;
            oldVoucher.PhanTramGiam = model.PhanTramGiam;
            oldVoucher.GiamToiDa = model.GiamToiDa;
            oldVoucher.DonToiThieu = model.DonToiThieu;
            oldVoucher.NgayBatDau = model.NgayBatDau;
            oldVoucher.NgayKetThuc = model.NgayKetThuc;
            oldVoucher.TrangThai = model.TrangThai;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}