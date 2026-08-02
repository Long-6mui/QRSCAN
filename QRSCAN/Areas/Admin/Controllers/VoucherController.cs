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
            var vouchers = _context.Vouchers.OrderByDescending(v => v.MaVoucher).ToList();
            return View(vouchers);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Voucher model)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(model.MaCode)) errors.Add("MaCode", "Mã Code không được trống.");
            else if (_context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower())) errors.Add("MaCode", "Mã Code đã tồn tại!");

            if (string.IsNullOrWhiteSpace(model.TenVoucher)) errors.Add("TenVoucher", "Tên Voucher không được trống.");
            if (model.GiaTriGiam <= 0) errors.Add("GiaTriGiam", "Giá trị giảm phải > 0.");
            if (model.DieuKien < 0) errors.Add("DieuKien", "Điều kiện không hợp lệ.");
            if (model.SoLuong < 0) errors.Add("SoLuong", "Số lượng không hợp lệ.");
            if (model.BatDau >= model.KetThuc) errors.Add("KetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            bool isExist = _context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower());
            if (isExist)
                return Json(new { success = false, message = "Mã Code này đã tồn tại trong hệ thống!" });

            if (errors.Any()) return Json(new { success = false, errors = errors });

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
            if (oldVoucher == null) return Json(new { success = false, message = "Lỗi dữ liệu, không tìm thấy Voucher!" });

            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(model.MaCode)) errors.Add("MaCode", "Mã Code không được trống.");
            else if (_context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower() && v.MaVoucher != model.MaVoucher)) errors.Add("MaCode", "Mã Code đã tồn tại!");

            if (string.IsNullOrWhiteSpace(model.TenVoucher)) errors.Add("TenVoucher", "Tên Voucher không được trống.");
            if (model.GiaTriGiam <= 0) errors.Add("GiaTriGiam", "Giá trị giảm phải > 0.");
            if (model.DieuKien < 0) errors.Add("DieuKien", "Điều kiện không hợp lệ.");
            if (model.SoLuong < 0) errors.Add("SoLuong", "Số lượng không hợp lệ.");
            if (model.BatDau >= model.KetThuc) errors.Add("KetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            bool isExist = _context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower() && v.MaVoucher != model.MaVoucher);
            if (isExist)
                return Json(new { success = false, message = "Mã Code này đang được sử dụng cho Voucher khác!" });

            if (errors.Any()) return Json(new { success = false, errors = errors });

            oldVoucher.MaCode = model.MaCode;
            oldVoucher.TenVoucher = model.TenVoucher;
            oldVoucher.LoaiGiamGia = model.LoaiGiamGia;
            oldVoucher.GiaTriGiam = model.GiaTriGiam;
            oldVoucher.DieuKien = model.DieuKien;
            oldVoucher.BatDau = model.BatDau;
            oldVoucher.KetThuc = model.KetThuc;
            oldVoucher.SoLuong = model.SoLuong;
            oldVoucher.TrangThai = model.TrangThai;

            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}