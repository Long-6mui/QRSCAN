using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRSCAN.Data;
using QRSCAN.Models.Entities;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin")] // Tạm tắt authorize để bạn dễ chạy tool
    public class DataGeneratorController : Controller
    {
        private readonly AppDbContext _context;

        public DataGeneratorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Run()
        {
            // 1. Kiem tra xem da co du lieu chua (tranh an nhieu lan tao ra 20k, 30k don)
            if (_context.DonHangs.Count() > 5000)
            {
                return Content("Dữ liệu đã được tạo trước đó rồi! Hãy vào xem biểu đồ.");
            }

            Random rand = new Random();

            // 2. TẠO KHÁCH HÀNG ĐA DẠNG (50 người)
            var ho = new[] { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng" };
            var dem = new[] { "Văn", "Thị", "Minh", "Ngọc", "Hữu", "Đức", "Hải", "Thanh", "Tuấn", "Thùy" };
            var ten = new[] { "Anh", "Bình", "Châu", "Dũng", "Dương", "Hà", "Hải", "Hùng", "Hương", "Linh", "Long", "Nga", "Nam", "Phúc", "Quân", "Sơn", "Trang", "Tùng", "Yến", "Vy" };

            List<KhachHang> dsKhach = new List<KhachHang>();
            for (int i = 0; i < 50; i++)
            {
                string tenKH = $"{ho[rand.Next(ho.Length)]} {dem[rand.Next(dem.Length)]} {ten[rand.Next(ten.Length)]}";
                dsKhach.Add(new KhachHang
                {
                    HoTen = tenKH,
                    SDT = $"09{rand.Next(10000000, 99999999)}",
                    Email = $"khachhang{i}@gmail.com",
                    LoaiKhach = rand.Next(100) > 80 ? "VIP" : "Thành viên",
                    DiemTichLuy = rand.Next(0, 500),
                    TrangThai = "HoatDong",
                    TenDangNhap = $"user_{i}",
                    MatKhau = "123456"
                });
            }
            _context.KhachHangs.AddRange(dsKhach);
            _context.SaveChanges(); // Lưu để lấy ID thật

            // 3. TẠO DANH MỤC VÀ MÓN ĂN MỚI
            var dmSushi = new DanhMucMon { TenDM = "Sushi & Sashimi", TrangThai = "HoatDong" };
            var dmBBQ = new DanhMucMon { TenDM = "Thịt nướng BBQ", TrangThai = "HoatDong" };
            _context.DanhMucMons.AddRange(dmSushi, dmBBQ);
            _context.SaveChanges();

            List<MonAn> dsMon = new List<MonAn>
            {
                new MonAn { MaDM = dmSushi.MaDM, TenMon = "Sushi Cá Hồi", DonGia = 65000, TrangThai = "DangBan", SoLuong = 100 },
                new MonAn { MaDM = dmSushi.MaDM, TenMon = "Sashimi Tổng Hợp", DonGia = 180000, TrangThai = "DangBan", SoLuong = 100 },
                new MonAn { MaDM = dmSushi.MaDM, TenMon = "Maki Trứng Cá", DonGia = 45000, TrangThai = "DangBan", SoLuong = 100 },
                new MonAn { MaDM = dmBBQ.MaDM, TenMon = "Bò Wagyu Nướng", DonGia = 350000, TrangThai = "DangBan", SoLuong = 50 },
                new MonAn { MaDM = dmBBQ.MaDM, TenMon = "Ba Chỉ Bò Mỹ", DonGia = 120000, TrangThai = "DangBan", SoLuong = 50 }
            };
            _context.MonAns.AddRange(dsMon);
            _context.SaveChanges();

            // Lấy toàn bộ danh sách để Random
            var tatCaMonAn = _context.MonAns.ToList();
            var tatCaKhach = _context.KhachHangs.Select(k => k.MaKH).ToList();
            var tatCaBan = _context.BanAns.Select(b => b.MaBan).ToList();

            // Xử lý an toàn nếu Db chưa có Bàn ăn (Tạo 5 bàn ảo)
            if (!tatCaBan.Any())
            {
                for (int i = 1; i <= 5; i++) _context.BanAns.Add(new BanAn { TenBan = $"Bàn {i}", SoChoNgoi = 4, TrangThai = "Trong" });
                _context.SaveChanges();
                tatCaBan = _context.BanAns.Select(b => b.MaBan).ToList();
            }

            // 4. TẠO 10,000 ĐƠN HÀNG NGẪU NHIÊN TRONG 5 NĂM (2021 - 2026)
            DateTime endDate = new DateTime(2026, 7, 17);
            DateTime startDate = endDate.AddYears(-5);
            int totalDays = (endDate - startDate).Days;

            List<PhienGoiMon> batchPhien = new List<PhienGoiMon>();

            for (int i = 0; i < 10000; i++)
            {
                // Ngày giờ random
                DateTime randomDate = startDate.AddDays(rand.Next(totalDays)).AddHours(rand.Next(8, 22)).AddMinutes(rand.Next(0, 59));

                // 4.1. Tạo Phiên (Khách vào bàn)
                var phien = new PhienGoiMon
                {
                    MaKH = tatCaKhach[rand.Next(tatCaKhach.Count)],
                    MaBan = tatCaBan[rand.Next(tatCaBan.Count)],
                    BatDau = randomDate,
                    KetThuc = randomDate.AddMinutes(rand.Next(30, 120)), // Ăn từ 30 -> 120 phút
                    TrangThai = "Dong" // Đã ăn xong
                };

                // 4.2. Tạo Đơn hàng
                var donHang = new DonHang
                {
                    ThoiGianTao = randomDate.AddMinutes(5), // Đặt món sau khi ngồi 5 phút
                    TrangThai = "Hoàn thành",
                    SoTienGiam = 0,
                    TongTien = 0 // Tí tính sau
                };

                // 4.3. Tạo Chi tiết Đơn hàng (Mua random từ 1 đến 5 món)
                int soMonMua = rand.Next(1, 6);
                decimal tongTienDon = 0;

                for (int j = 0; j < soMonMua; j++)
                {
                    var monDuocChon = tatCaMonAn[rand.Next(tatCaMonAn.Count)];
                    int soLuong = rand.Next(1, 4); // Mỗi món mua từ 1-3 phần
                    decimal thanhTien = monDuocChon.DonGia * soLuong;

                    tongTienDon += thanhTien;

                    donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        MaMon = monDuocChon.MaMon,
                        SoLuong = soLuong,
                        DonGia = monDuocChon.DonGia,
                        ThanhTien = thanhTien,
                        TrangThai = "Hoàn thành"
                    });
                }

                // Cập nhật lại tổng tiền cho đơn
                donHang.TongTien = tongTienDon;

                // 4.4. Tạo Hóa Đơn (Thanh toán)
                donHang.HoaDons.Add(new HoaDon
                {
                    MaPT = rand.Next(1, 3), // 1: Tiền mặt, 2: Chuyển khoản
                    ThoiGianTao = phien.KetThuc.Value, // Thanh toán lúc ra về
                    TongTien = tongTienDon,
                    SoTienGiam = 0,
                    TrangThai = "Đã thanh toán"
                });

                phien.DonHangs.Add(donHang);
                batchPhien.Add(phien);

                // Lưu theo từng cục 1000 đơn để không bị tràn RAM
                if (batchPhien.Count >= 1000)
                {
                    _context.PhienGoiMons.AddRange(batchPhien);
                    _context.SaveChanges();
                    batchPhien.Clear(); // Dọn rác
                }
            }

            // Lưu phần dư còn lại
            if (batchPhien.Any())
            {
                _context.PhienGoiMons.AddRange(batchPhien);
                _context.SaveChanges();
            }

            return Content("Thành công! Đã tạo xong 10,000 hóa đơn cùng hàng ngàn món ăn, khách hàng. Bạn có thể quay lại trang Thống kê để xem biểu đồ.");
        }
    }
}