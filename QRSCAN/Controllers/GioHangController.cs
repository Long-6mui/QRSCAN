using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;
using QRSCAN.Models.Entities;
using QRSCAN.ViewModels;
using System.Text.Json;

namespace QRSCAN.Controllers
{
    public class GioHangController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public GioHangController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private List<GioHangItemViewModel> LayGioHang()
        {
            var json = HttpContext.Session.GetString("GioHang");

            if (string.IsNullOrEmpty(json))
            {
                return new List<GioHangItemViewModel>();
            }

            return JsonSerializer.Deserialize<List<GioHangItemViewModel>>(json)
                   ?? new List<GioHangItemViewModel>();
        }

        private void LuuGioHang(List<GioHangItemViewModel> gioHang)
        {
            var json = JsonSerializer.Serialize(gioHang);
            HttpContext.Session.SetString("GioHang", json);
        }

        private void XoaVoucher()
        {
            HttpContext.Session.Remove("MaVoucher");
            HttpContext.Session.Remove("MaVoucherCode");
            HttpContext.Session.Remove("TienGiam");
        }

        public IActionResult Index()
        {
            var gioHang = LayGioHang();

            var tongTien = gioHang.Sum(x => x.SoLuong * x.DonGia);

            decimal tienGiam = 0;
            var tienGiamSession = HttpContext.Session.GetString("TienGiam");

            if (!string.IsNullOrEmpty(tienGiamSession))
            {
                decimal.TryParse(tienGiamSession, out tienGiam);
            }

            ViewBag.TongTien = tongTien;
            ViewBag.TienGiam = tienGiam;
            ViewBag.TongThanhToan = tongTien - tienGiam;
            ViewBag.MaVoucherCode = HttpContext.Session.GetString("MaVoucherCode");

            return View(gioHang);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int maMon)
        {
            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(x => x.MaMon == maMon && x.TrangThai == "DangBan");

            if (monAn == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy món ăn."
                });
            }

            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(x => x.MaMon == maMon);

            if (item == null)
            {
                gioHang.Add(new GioHangItemViewModel
                {
                    MaMon = monAn.MaMon,
                    TenMon = monAn.TenMon,
                    HinhAnh = monAn.HinhAnh,
                    DonGia = monAn.DonGia,
                    SoLuong = 1
                });
            }
            else
            {
                item.SoLuong++;
            }

            LuuGioHang(gioHang);
            XoaVoucher();

            return Json(new
            {
                success = true,
                message = "Đã thêm món vào giỏ hàng.",
                soLuong = gioHang.Sum(x => x.SoLuong)
            });
        }

        [HttpPost]
        public IActionResult TangSoLuong(int maMon)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(x => x.MaMon == maMon);

            if (item != null)
            {
                item.SoLuong++;
            }

            LuuGioHang(gioHang);
            XoaVoucher();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GiamSoLuong(int maMon)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(x => x.MaMon == maMon);

            if (item != null)
            {
                item.SoLuong--;

                if (item.SoLuong <= 0)
                {
                    gioHang.Remove(item);
                }
            }

            LuuGioHang(gioHang);
            XoaVoucher();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaMon(int maMon)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(x => x.MaMon == maMon);

            if (item != null)
            {
                gioHang.Remove(item);
            }

            LuuGioHang(gioHang);
            XoaVoucher();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApDungVoucher(string maCode)
        {
            var gioHang = LayGioHang();

            if (!gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(maCode))
            {
                TempData["Error"] = "Vui lòng nhập mã voucher.";
                return RedirectToAction("Index");
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(x =>
                    x.MaCode == maCode.Trim()
                    && x.TrangThai == "HoatDong"
                    && x.BatDau <= DateTime.Now
                    && x.KetThuc >= DateTime.Now
                    && x.SoLuong > 0);

            if (voucher == null)
            {
                TempData["Error"] = "Voucher không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            var tongTien = gioHang.Sum(x => x.SoLuong * x.DonGia);

            if (tongTien < voucher.DieuKien)
            {
                TempData["Error"] = $"Đơn hàng chưa đủ điều kiện áp dụng voucher. Tối thiểu {voucher.DieuKien:N0}đ.";
                return RedirectToAction("Index");
            }

            decimal tienGiam;

            if (voucher.LoaiGiamGia == "PhanTram")
            {
                tienGiam = tongTien * voucher.GiaTriGiam / 100;
            }
            else
            {
                tienGiam = voucher.GiaTriGiam;
            }

            if (tienGiam > tongTien)
            {
                tienGiam = tongTien;
            }

            HttpContext.Session.SetInt32("MaVoucher", voucher.MaVoucher);
            HttpContext.Session.SetString("MaVoucherCode", voucher.MaCode);
            HttpContext.Session.SetString("TienGiam", tienGiam.ToString());

            TempData["Success"] = "Áp dụng voucher thành công.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult HuyVoucher()
        {
            XoaVoucher();
            TempData["Success"] = "Đã hủy voucher.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GuiDon(string phuongThucThanhToan = "TienMat")
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            var maBan = HttpContext.Session.GetInt32("MaBan");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (maBan == null)
            {
                TempData["Error"] = "Bạn chưa quét mã QR bàn.";
                return RedirectToAction("Index");
            }

            var gioHang = LayGioHang();

            if (!gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            var tongTien = gioHang.Sum(x => x.SoLuong * x.DonGia);

            decimal tienGiam = 0;
            var tienGiamSession = HttpContext.Session.GetString("TienGiam");

            if (!string.IsNullOrEmpty(tienGiamSession))
            {
                decimal.TryParse(tienGiamSession, out tienGiam);
            }

            var maVoucher = HttpContext.Session.GetInt32("MaVoucher");

            var phien = await _context.PhienGoiMons
                .FirstOrDefaultAsync(x =>
                    x.MaKH == maKH.Value
                    && x.MaBan == maBan.Value
                    && x.TrangThai == "DangMo");

            if (phien == null)
            {
                phien = new PhienGoiMon
                {
                    MaKH = maKH.Value,
                    MaBan = maBan.Value,
                    BatDau = DateTime.Now,
                    KetThuc = null,
                    TrangThai = "DangMo"
                };

                _context.PhienGoiMons.Add(phien);
                await _context.SaveChangesAsync();
            }

            var donHang = new DonHang
            {
                MaPhien = phien.MaPhien,
                MaVoucher = maVoucher,
                ThoiGianTao = DateTime.Now,
                TongTien = tongTien,
                SoTienGiam = tienGiam,
                TrangThai = "ChoXacNhan"
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDH = donHang.MaDH,
                    MaMon = item.MaMon,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    ThanhTien = item.SoLuong * item.DonGia,
                    GhiChu = null,
                    TrangThai = "ChoCheBien"
                };

                _context.ChiTietDonHangs.Add(chiTiet);
            }

            var tenPT = phuongThucThanhToan == "QR" ? "Chuyen khoan" : "Tien mat";

            var pt = await _context.PhuongThucThanhToans
                .FirstOrDefaultAsync(x => x.TenPT == tenPT);

            var hoaDon = new HoaDon
            {
                MaDH = donHang.MaDH,
                MaNV = null,
                MaPT = pt?.MaPT ?? 1,
                ThoiGianTao = DateTime.Now,
                TongTien = donHang.TongTien,
                SoTienGiam = donHang.SoTienGiam,
                TrangThai = "ChuaThanhToan"
            };

            _context.HoaDons.Add(hoaDon);

            if (maVoucher != null)
            {
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(x => x.MaVoucher == maVoucher.Value);

                if (voucher != null && voucher.SoLuong > 0)
                {
                    voucher.SoLuong--;
                }
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("GioHang");
            XoaVoucher();

            if (tenPT == "Chuyen khoan")
            {
                return RedirectToAction("ThanhToanQR", new { maDH = donHang.MaDH });
            }

            return RedirectToAction("DatHangThanhCong", new { maDH = donHang.MaDH });
        }

        public async Task<IActionResult> ThanhToanQR(int maDH)
        {
            var donHang = await _context.DonHangs
                .Include(x => x.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .FirstOrDefaultAsync(x => x.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.PhuongThucThanhToan)
                .Where(h => h.MaDH == maDH)
                .OrderByDescending(h => h.MaHD)
                .FirstOrDefaultAsync();

            if (hoaDon == null || hoaDon.PhuongThucThanhToan?.TenPT != "Chuyen khoan")
            {
                return RedirectToAction("DatHangThanhCong", new { maDH = donHang.MaDH });
            }

            if (hoaDon.TrangThai == "DaThanhToan")
            {
                return RedirectToAction("DatHangThanhCong", new { maDH = donHang.MaDH });
            }

            var tongThanhToan = donHang.TongTien - donHang.SoTienGiam;

            var bankBin = _configuration["BankInfo:BankBin"];
            var soTaiKhoan = _configuration["BankInfo:AccountNumber"];
            var tenTaiKhoan = _configuration["BankInfo:AccountName"];
            var noiDung = $"DH{donHang.MaDH} HD{hoaDon.MaHD}";

            var qrUrl =
                $"https://img.vietqr.io/image/{bankBin}-{soTaiKhoan}-qr_only.png" +
                $"?amount={(long)tongThanhToan}" +
                $"&addInfo={Uri.EscapeDataString(noiDung)}" +
                $"&accountName={Uri.EscapeDataString(tenTaiKhoan ?? string.Empty)}";

            ViewBag.MaDH = donHang.MaDH;
            ViewBag.MaBan = donHang.PhienGoiMon?.MaBan;
            ViewBag.TenBan = donHang.PhienGoiMon?.BanAn?.TenBan;
            ViewBag.TongThanhToan = tongThanhToan;
            ViewBag.QrImageUrl = qrUrl;
            ViewBag.SoTaiKhoan = soTaiKhoan;
            ViewBag.TenTaiKhoan = tenTaiKhoan;
            ViewBag.NoiDung = noiDung;

            var hetHan = hoaDon.ThoiGianTao.AddMinutes(2);
            ViewBag.QrHetHanTs = new DateTimeOffset(hetHan).ToUnixTimeMilliseconds();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanThanhToanQR(int maDH)
        {
            var hoaDon = await _context.HoaDons
                .Where(h => h.MaDH == maDH)
                .OrderByDescending(h => h.MaHD)
                .FirstOrDefaultAsync();

            if (hoaDon != null)
            {
                hoaDon.TrangThai = "DaThanhToan";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DatHangThanhCong", new { maDH = maDH });
        }

        public async Task<IActionResult> DatHangThanhCong(int maDH)
        {
            var donHang = await _context.DonHangs
                .Include(x => x.PhienGoiMon)
                    .ThenInclude(p => p!.BanAn)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.MaDH == maDH);

            if (donHang == null)
            {
                return NotFound();
            }

            ViewBag.MaDH = donHang.MaDH;
            ViewBag.MaBan = donHang.PhienGoiMon?.MaBan;
            ViewBag.TenBan = donHang.PhienGoiMon?.BanAn?.TenBan;
            ViewBag.TrangThai = donHang.TrangThai;
            ViewBag.TongTien = donHang.TongTien;
            ViewBag.TienGiam = donHang.SoTienGiam;
            ViewBag.TongThanhToan = donHang.TongTien - donHang.SoTienGiam;
            ViewBag.MaVoucherCode = donHang.Voucher?.MaCode;

            return View();
        }
    }
}