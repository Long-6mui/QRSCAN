using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Controllers
{
    public class ChefController : Controller
    {
        private readonly AppDbContext _context;

        public ChefController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsBepDangNhap()
        {
            var maVT = HttpContext.Session.GetInt32("MaVT");
            var tenVaiTro = HttpContext.Session.GetString("TenVaiTro");

            return maVT == 2 || tenVaiTro == "Bep";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsBepDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSachDonHang = await _context.DonHangs
                .Include(d => d.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MonAn)
                .Where(d => d.TrangThai == "ChoXacNhan"
                    || d.TrangThai == "DangCheBien")
                .OrderBy(d => d.ThoiGianTao)
                .ToListAsync();

            return View(danhSachDonHang);
        }

        [HttpPost]
        public async Task<IActionResult> NhanDon(int maDH)
        {
            if (!IsBepDangNhap())
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

            donHang.TrangThai = "DangCheBien";

            foreach (var ct in donHang.ChiTietDonHangs)
            {
                ct.TrangThai = "DangCheBien";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> HoanThanhMon(int maCT_DH)
        {
            if (!IsBepDangNhap())
            {
                return RedirectToAction("Login", "Account");
            }

            var chiTiet = await _context.ChiTietDonHangs
                .Include(ct => ct.DonHang)
                    .ThenInclude(d => d!.ChiTietDonHangs)
                .FirstOrDefaultAsync(ct => ct.MaCT_DH == maCT_DH);

            if (chiTiet == null)
            {
                return NotFound();
            }

            chiTiet.TrangThai = "HoanThanh";

            if (chiTiet.DonHang != null)
            {
                var tatCaDaXong = chiTiet.DonHang.ChiTietDonHangs
                    .All(ct => ct.TrangThai == "HoanThanh");

                if (tatCaDaXong)
                {
                    chiTiet.DonHang.TrangThai = "DaPhucVu";
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> HoanThanhDon(int maDH)
        {
            if (!IsBepDangNhap())
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

            donHang.TrangThai = "DaPhucVu";

            foreach (var ct in donHang.ChiTietDonHangs)
            {
                ct.TrangThai = "HoanThanh";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}