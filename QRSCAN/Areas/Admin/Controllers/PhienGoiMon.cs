using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Phục vụ")]
    public class PhienGoiMonController : Controller
    {
        private readonly AppDbContext _context;

        public PhienGoiMonController(AppDbContext context)
        {
            _context = context;
        }

        public class PhienGoiMonViewModel
        {
            public int MaBan { get; set; }
            public string? TenBan { get; set; }
            public string? TrangThaiBan { get; set; }
            public int SoChoNgoi { get; set; }

            public int? MaDonHangThucTe { get; set; }
            public DateTime? ThoiGianVao { get; set; }
            public decimal? TongTienHienTai { get; set; }
        }

        public IActionResult Index()
        {
            var tables = _context.BanAns.OrderBy(b => b.TenBan).ToList();

            var activeOrders = _context.DonHangs
                .Where(d => d.TrangThai != "Hoàn thành" && d.TrangThai != "Đã hủy" && d.MaBan != null)
                .ToList();

            var model = new List<PhienGoiMonViewModel>();

            foreach (var ban in tables)
            {
                var vm = new PhienGoiMonViewModel
                {
                    MaBan = ban.MaBan,
                    TenBan = ban.TenBan,
                    TrangThaiBan = ban.TrangThai,
                    SoChoNgoi = ban.SoChoNgoi
                };

                if (ban.TrangThai == "Đang phục vụ")
                {
                    var donHienTai = activeOrders.OrderByDescending(d => d.ThoiGianDat).FirstOrDefault(d => d.MaBan == ban.MaBan);
                    if (donHienTai != null)
                    {
                        vm.MaDonHangThucTe = donHienTai.MaDonHang;
                        vm.ThoiGianVao = donHienTai.ThoiGianDat;
                        vm.TongTienHienTai = donHienTai.TongThanhToan;
                    }
                }

                model.Add(vm);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GetChiTiet(int maDonHang)
        {
            var donHang = _context.DonHangs.Find(maDonHang);
            if (donHang == null) return NotFound();

            var chiTiets = _context.ChiTietDonHangs
                .Include(c => c.MonAn)
                .Where(c => c.MaDonHang == maDonHang)
                .Select(c => new
                {
                    tenMon = c.MonAn != null ? c.MonAn.TenMon : "Món đã xóa",
                    soLuong = c.SoLuong,
                    donGia = c.DonGia,
                    thanhTien = c.ThanhTien
                }).ToList();

            return Json(new
            {
                success = true,
                maDonHang = donHang.MaDonHang,
                thoiGian = donHang.ThoiGianDat.ToString("HH:mm dd/MM/yyyy"),
                trangThai = donHang.TrangThai,
                tongThanhToan = donHang.TongThanhToan,
                chiTiets = chiTiets
            });
        }

        [HttpGet]
        public IActionResult GetBanTrong()
        {
            var banTrongs = _context.BanAns
                .Where(b => b.TrangThai == "Trống")
                .Select(b => new { b.MaBan, b.TenBan }).ToList();
            return Json(new { success = true, data = banTrongs });
        }

        public class DoiBanRequest
        {
            public int MaDonHang { get; set; }
            public int MaBanMoi { get; set; }
        }

        [HttpPost]
        public IActionResult DoiBan([FromBody] DoiBanRequest request)
        {
            var donHang = _context.DonHangs.Find(request.MaDonHang);
            if (donHang == null) return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });

            if (donHang.MaBan.HasValue)
            {
                var banCu = _context.BanAns.Find(donHang.MaBan.Value);
                if (banCu != null) banCu.TrangThai = "Trống";
            }

            var banMoi = _context.BanAns.Find(request.MaBanMoi);
            if (banMoi == null) return Json(new { success = false, message = "Bàn mới không tồn tại!" });
            banMoi.TrangThai = "Đang phục vụ";

            donHang.MaBan = request.MaBanMoi;
            _context.SaveChanges();

            return Json(new { success = true, message = "Chuyển bàn thành công!" });
        }
    }
}