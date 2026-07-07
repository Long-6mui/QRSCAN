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

        public GioHangController(AppDbContext context)
        {
            _context = context;
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
            HttpContext.Session.Remove("TenVoucher");
            HttpContext.Session.Remove("TienGiam");
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int maMon)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Bạn cần đăng nhập để thêm món vào giỏ hàng."
                });
            }

            var mon = await _context.MonAns
                .FirstOrDefaultAsync(m => m.MaMon == maMon && m.TrangThai == "DangBan");

            if (mon == null)
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
                    MaMon = mon.MaMon,
                    TenMon = mon.TenMon,
                    DonGia = mon.DonGia,
                    HinhAnh = mon.HinhAnh,
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
                message = "Đã thêm món vào giỏ hàng",
                cartCount = gioHang.Sum(x => x.SoLuong)
            });
        }

        public IActionResult Index()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = LayGioHang();

            var tongTien = gioHang.Sum(x => x.ThanhTien);
            var tienGiam = HttpContext.Session.GetString("TienGiam");

            decimal tienGiamValue = 0;

            if (!string.IsNullOrEmpty(tienGiam))
            {
                decimal.TryParse(tienGiam, out tienGiamValue);
            }

            if (tienGiamValue > tongTien)
            {
                tienGiamValue = tongTien;
            }

            ViewBag.TongTien = tongTien;
            ViewBag.TienGiam = tienGiamValue;
            ViewBag.TongThanhToan = tongTien - tienGiamValue;
            ViewBag.MaBan = HttpContext.Session.GetInt32("MaBan");
            ViewBag.MaVoucherCode = HttpContext.Session.GetString("MaVoucherCode");
            ViewBag.TenVoucher = HttpContext.Session.GetString("TenVoucher");

            return View(gioHang);
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
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = LayGioHang();

            if (gioHang.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(maCode))
            {
                TempData["Error"] = "Vui lòng nhập mã voucher.";
                return RedirectToAction("Index");
            }

            maCode = maCode.Trim().ToUpper();

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(x => x.MaCode == maCode && x.TrangThai == "HoatDong");

            if (voucher == null)
            {
                TempData["Error"] = "Mã voucher không hợp lệ.";
                return RedirectToAction("Index");
            }

            var today = DateTime.Now.Date;

            if (today < voucher.NgayBatDau.Date || today > voucher.NgayKetThuc.Date)
            {
                TempData["Error"] = "Voucher đã hết hạn hoặc chưa đến thời gian sử dụng.";
                return RedirectToAction("Index");
            }

            var tongTien = gioHang.Sum(x => x.ThanhTien);

            if (tongTien < voucher.DonToiThieu)
            {
                TempData["Error"] = $"Đơn hàng phải từ {voucher.DonToiThieu:N0}đ để dùng voucher này.";
                return RedirectToAction("Index");
            }

            var tienGiam = tongTien * voucher.PhanTramGiam / 100;

            if (tienGiam > voucher.GiamToiDa)
            {
                tienGiam = voucher.GiamToiDa;
            }

            HttpContext.Session.SetInt32("MaVoucher", voucher.MaVoucher);
            HttpContext.Session.SetString("MaVoucherCode", voucher.MaCode);
            HttpContext.Session.SetString("TenVoucher", voucher.TenVoucher);
            HttpContext.Session.SetString("TienGiam", tienGiam.ToString());

            TempData["Success"] = $"Áp dụng voucher {voucher.MaCode} thành công.";

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
        public async Task<IActionResult> GuiDon(string phuongThucThanhToan)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            var maBan = HttpContext.Session.GetInt32("MaBan");

            // Chỉ chấp nhận 2 phương thức: QR hoặc Tiền mặt
            if (phuongThucThanhToan != "QR" && phuongThucThanhToan != "TienMat")
            {
                phuongThucThanhToan = "TienMat";
            }

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (maBan == null)
            {
                TempData["Error"] = "Chưa xác định bàn. Vui lòng quét mã QR tại bàn.";
                return RedirectToAction("Index");
            }

            var gioHang = LayGioHang();

            if (gioHang.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            var tongTien = gioHang.Sum(x => x.ThanhTien);

            var maVoucher = HttpContext.Session.GetInt32("MaVoucher");
            var maVoucherCode = HttpContext.Session.GetString("MaVoucherCode");
            var tienGiamString = HttpContext.Session.GetString("TienGiam");

            decimal tienGiam = 0;

            if (!string.IsNullOrEmpty(tienGiamString))
            {
                decimal.TryParse(tienGiamString, out tienGiam);
            }

            if (tienGiam > tongTien)
            {
                tienGiam = tongTien;
            }

            var tongThanhToan = tongTien - tienGiam;

            var donHang = new DonHang
            {
                MaKH = maKH.Value,
                MaBan = maBan.Value,
                ThoiGianDat = DateTime.Now,
                TrangThai = "ChoXacNhan",
                PhuongThucThanhToan = phuongThucThanhToan,
                TongTien = tongTien,
                MaVoucher = maVoucher,
                MaVoucherCode = maVoucherCode,
                TienGiam = tienGiam,
                TongThanhToan = tongThanhToan
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaMon = item.MaMon,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    ThanhTien = item.ThanhTien
                };

                _context.ChiTietDonHangs.Add(chiTiet);
            }

            // Tạo bản ghi thanh toán tương ứng với đơn hàng vừa tạo
            var thanhToan = new ThanhToan
            {
                MaDonHang = donHang.MaDonHang,
                PhuongThuc = phuongThucThanhToan,
                SoTien = tongThanhToan,
                // Thanh toán tiền mặt sẽ được nhân viên thu và xác nhận trực tiếp,
                // thanh toán QR chờ khách quét mã và chuyển khoản
                TrangThai = "ChoThanhToan",
                NgayThanhToan = DateTime.Now
            };

            _context.ThanhToans.Add(thanhToan);

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("GioHang");
            XoaVoucher();

            return RedirectToAction("DatHangThanhCong", new { maDonHang = donHang.MaDonHang });
        }

        public async Task<IActionResult> DatHangThanhCong(int maDonHang)
        {
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);

            if (donHang == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var thanhToan = await _context.ThanhToans
                .Where(x => x.MaDonHang == maDonHang)
                .OrderByDescending(x => x.MaThanhToan)
                .FirstOrDefaultAsync();

            ViewBag.MaDonHang = donHang.MaDonHang;
            ViewBag.MaBan = donHang.MaBan;
            ViewBag.TongTien = donHang.TongTien;
            ViewBag.TienGiam = donHang.TienGiam;
            ViewBag.TongThanhToan = donHang.TongThanhToan;
            ViewBag.MaVoucherCode = donHang.MaVoucherCode;
            ViewBag.TrangThai = donHang.TrangThai;
            ViewBag.PhuongThucThanhToan = donHang.PhuongThucThanhToan ?? "TienMat";
            ViewBag.TrangThaiThanhToan = thanhToan?.TrangThai ?? "ChoThanhToan";

            if (ViewBag.PhuongThucThanhToan == "QR")
            {
                // Nội dung mã QR chuyển khoản: mã đơn + số tiền + bàn, dùng để nhân viên/khách đối chiếu
                var noiDungQR = $"KIMURA RAMEN - Don hang #{donHang.MaDonHang} - Ban {donHang.MaBan} - So tien {donHang.TongThanhToan:N0}d";
                ViewBag.QRCodeUrl = "https://api.qrserver.com/v1/create-qr-code/?size=240x240&data=" + Uri.EscapeDataString(noiDungQR);
            }

            return View();
        }

        // Xác nhận đơn hàng đã được thanh toán (QR: khách/nhân viên xác nhận sau khi chuyển khoản,
        // Tiền mặt: nhân viên xác nhận sau khi thu tiền tại bàn)
        [HttpPost]
        public async Task<IActionResult> XacNhanThanhToan(int maDonHang)
        {
            var thanhToan = await _context.ThanhToans
                .Where(x => x.MaDonHang == maDonHang)
                .OrderByDescending(x => x.MaThanhToan)
                .FirstOrDefaultAsync();

            if (thanhToan != null)
            {
                thanhToan.TrangThai = "DaThanhToan";
                thanhToan.NgayThanhToan = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DatHangThanhCong", new { maDonHang });
        }


    }
}