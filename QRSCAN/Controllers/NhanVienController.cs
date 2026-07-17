using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.ViewModels;

namespace QRSCAN.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly AppDbContext _context;

        public NhanVienController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsNhanVienPhucVuDangNhap()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");
            var maVT = HttpContext.Session.GetInt32("MaVT");
            var tenVaiTro = HttpContext.Session.GetString("TenVaiTro");

            return maNV != null
                   && (maVT == 1
                       
                       || tenVaiTro == "NhanVien"
                       || tenVaiTro == "NhanVienPhucVu"
                       || tenVaiTro == "NhanVienQuay"
                    );
        }

        public async Task<IActionResult> Index()
        {
            if (!IsNhanVienPhucVuDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangsCanXuLy = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .Where(d => d.TrangThai == "ChoXacNhan"
                    || d.TrangThai == "ChoThanhToan"
                    || d.TrangThai == "DangCheBien"
                    || d.TrangThai == "DaPhucVu")
                .OrderBy(d => d.ThoiGianTao)
                .ToListAsync();

            var banAns = await _context.BanAns
                .OrderBy(b => b.MaBan)
                .ToListAsync();

            var phienDangMo = await _context.PhienGoiMons
                .Include(p => p.BanAn)
                .Include(p => p.DonHangs)
                    .ThenInclude(d => d.HoaDons)
                .Where(p => p.TrangThai == "DangMo")
                .ToListAsync();

            var banTrangThai = banAns.Select(ban =>
            {
                var phien = phienDangMo
                    .FirstOrDefault(p => p.MaBan == ban.MaBan);

                if (phien == null)
                {
                    return new BanTrangThaiViewModel
                    {
                        MaBan = ban.MaBan,
                        TenBan = ban.TenBan,
                        TrangThai = "Trong"
                    };
                }

                var donMoiNhat = phien.DonHangs
                    .OrderByDescending(d => d.ThoiGianTao)
                    .FirstOrDefault();

                var trangThaiBan = "DangPhucVu";

                if (donMoiNhat?.TrangThai == "ChoThanhToan")
                {
                    trangThaiBan = "ChoThanhToan";
                }

                return new BanTrangThaiViewModel
                {
                    MaBan = ban.MaBan,
                    TenBan = ban.TenBan,
                    TrangThai = trangThaiBan,
                    MaPhien = phien.MaPhien,
                    MaDH = donMoiNhat?.MaDH,
                    TongTien = donMoiNhat == null
                        ? null
                        : donMoiNhat.TongTien - donMoiNhat.SoTienGiam
                };
            }).ToList();

            var viewModel = new NhanVienDashboardViewModel
            {
                DonHangsCanXuLy = donHangsCanXuLy,
                BanAns = banTrangThai
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ChiTietDonHang(int maDH)
        {
            if (!IsNhanVienPhucVuDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .Include(d => d.HoaDons)
                    .ThenInclude(h => h.PhuongThucThanhToan)
                .FirstOrDefaultAsync(d => d.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        public async Task<IActionResult> ChiTietBan(int maBan)
        {
            if (!IsNhanVienPhucVuDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var ban = await _context.BanAns
                .FirstOrDefaultAsync(b => b.MaBan == maBan);

            if (ban == null)
            {
                return NotFound();
            }

            var phienDangMo = await _context.PhienGoiMons
                .Include(p => p.BanAn)
                .Include(p => p.DonHangs)
                    .ThenInclude(d => d.ChiTietDonHangs)
                        .ThenInclude(ct => ct.MonAn)
                .Include(p => p.DonHangs)
                    .ThenInclude(d => d.HoaDons)
                        .ThenInclude(h => h.PhuongThucThanhToan)
                .Where(p => p.MaBan == maBan && p.TrangThai == "DangMo")
                .OrderByDescending(p => p.BatDau)
                .FirstOrDefaultAsync();

            ViewBag.Ban = ban;

            return View(phienDangMo);
        }

        [HttpPost]
        public async Task<IActionResult> TiepNhanDon(int maDH)
        {
            if (!IsNhanVienPhucVuDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            if (donHang.TrangThai == "ChoXacNhan")
            {
                donHang.TrangThai = "ChoThanhToan";

                foreach (var ct in donHang.ChiTietDonHangs)
                {
                    ct.TrangThai = "ChoThanhToan";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanDaPhucVu(int maDH)
        {
            if (!IsNhanVienPhucVuDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .Include(d => d.PhienGoiMon)
                .FirstOrDefaultAsync(d => d.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            if (donHang.TrangThai == "DaPhucVu")
            {
                donHang.TrangThai = "HoanThanh";

                foreach (var ct in donHang.ChiTietDonHangs)
                {
                    ct.TrangThai = "DaPhucVu";
                }

                if (donHang.PhienGoiMon != null)
                {
                    donHang.PhienGoiMon.TrangThai = "DaDong";
                    donHang.PhienGoiMon.KetThuc = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}