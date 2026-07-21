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
            public string TenBan { get; set; }
            public string TrangThaiBan { get; set; }
            public int SoChoNgoi { get; set; }

            // Dùng MaPhien làm gốc
            public int? MaPhien { get; set; }
            public DateTime? BatDau { get; set; } // Khớp với model PhienGoiMon
            public decimal? TongTienHienTai { get; set; }
        }

        public IActionResult Index()
        {
            var tables = _context.BanAns.OrderBy(b => b.TenBan).ToList();

            // 1. Lấy các Phiên đang mở
            var activeSessions = _context.PhienGoiMons
                .Where(p => p.TrangThai == "DangMo")
                .ToList();

            var activeSessionIds = activeSessions.Select(p => p.MaPhien).ToList();

            // 2. Lấy Đơn hàng (MaDH) thuộc Phiên (MaPhien)
            var activeOrders = _context.DonHangs
                .Where(d => activeSessionIds.Contains(d.MaPhien))
                .ToList();

            var activeOrderIds = activeOrders.Select(d => d.MaDH).ToList();

            // 3. Lấy Chi Tiết (MaCT_DH) thuộc Đơn hàng (MaDH)
            var activeChiTiets = _context.ChiTietDonHangs
                .Where(c => activeOrderIds.Contains(c.MaDH))
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

                // Trạng thái trong DB của bạn là "DangPhucVu" hoặc "Trong" (không dấu)
                if (ban.TrangThai != "Trong")
                {
                    var phienHienTai = activeSessions.FirstOrDefault(p => p.MaBan == ban.MaBan);

                    if (phienHienTai != null)
                    {
                        vm.MaPhien = phienHienTai.MaPhien;
                        vm.BatDau = phienHienTai.BatDau;

                        var orderIdsOfSession = activeOrders.Where(d => d.MaPhien == phienHienTai.MaPhien).Select(d => d.MaDH).ToList();

                        vm.TongTienHienTai = activeChiTiets
                            .Where(c => orderIdsOfSession.Contains(c.MaDH))
                            .Sum(c => c.ThanhTien);
                    }
                }
                model.Add(vm);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GetChiTiet(int maPhien)
        {
            var donHangs = _context.DonHangs.Where(d => d.MaPhien == maPhien).Select(d => d.MaDH).ToList();

            var chiTiets = _context.ChiTietDonHangs
                .Include(c => c.MonAn)
                .Where(c => donHangs.Contains(c.MaDH))
                .GroupBy(c => c.MonAn != null ? c.MonAn.TenMon : "Món đã xóa")
                .Select(g => new
                {
                    tenMon = g.Key,
                    soLuong = g.Sum(c => c.SoLuong),
                    thanhTien = g.Sum(c => c.ThanhTien)
                }).ToList();

            return Json(new
            {
                success = true,
                chiTiets = chiTiets,
                tongThanhToan = chiTiets.Sum(c => c.thanhTien)
            });
        }

        [HttpGet]
        public IActionResult GetBanTrong()
        {
            var banTrongs = _context.BanAns
                .Where(b => b.TrangThai == "Trong")
                .Select(b => new { b.MaBan, b.TenBan })
                .ToList();

            return Json(new { success = true, data = banTrongs });
        }

        public class DoiBanRequest
        {
            public int MaPhien { get; set; }
            public int MaBanMoi { get; set; }
        }

        [HttpPost]
        public IActionResult DoiBan([FromBody] DoiBanRequest request)
        {
            var phien = _context.PhienGoiMons.Find(request.MaPhien);
            if (phien == null) return Json(new { success = false, message = "Không tìm thấy Phiên gọi món!" });

            // Cập nhật Bàn cũ
            var banCu = _context.BanAns.Find(phien.MaBan);
            if (banCu != null) banCu.TrangThai = "Trong";

            // Cập nhật Bàn mới
            var banMoi = _context.BanAns.Find(request.MaBanMoi);
            if (banMoi == null) return Json(new { success = false, message = "Bàn mới không tồn tại!" });
            banMoi.TrangThai = "DangPhucVu";

            // Chuyển liên kết bàn của Phiên
            phien.MaBan = request.MaBanMoi;

            _context.SaveChanges();

            return Json(new { success = true, message = "Chuyển bàn thành công!" });
        }
    }
}