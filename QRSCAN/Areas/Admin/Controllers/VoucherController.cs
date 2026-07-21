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
            if (string.IsNullOrEmpty(model.MaCode))
                return Json(new { success = false, message = "Mã Code không được để trống!" });

            // Sử dụng trường BatDau và KetThuc khớp với Model
            if (model.BatDau >= model.KetThuc)
                return Json(new { success = false, message = "Ngày kết thúc phải lớn hơn ngày bắt đầu!" });

            // Kiểm tra trùng mã Code
            bool isExist = _context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower());
            if (isExist)
                return Json(new { success = false, message = "Mã Code này đã tồn tại trong hệ thống!" });

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

            // Kiểm tra trùng mã Code với các Voucher khác
            bool isExist = _context.Vouchers.Any(v => v.MaCode.ToLower() == model.MaCode.ToLower() && v.MaVoucher != model.MaVoucher);
            if (isExist)
                return Json(new { success = false, message = "Mã Code này đang được sử dụng cho Voucher khác!" });

            // Cập nhật chuẩn các trường theo Model Voucher.cs
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