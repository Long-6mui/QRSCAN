using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRSCAN.Data;

namespace QRSCAN.Controllers
{
	public class UserController : Controller
	{
		private readonly AppDbContext _context;

		public UserController(AppDbContext context)
		{
			_context = context;
		}

		// 1. Hiển thị thông tin hồ sơ khách hàng
		public async Task<IActionResult> Profile()
		{
			var maKH = HttpContext.Session.GetInt32("MaKH");

			if (maKH == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var khachHang = await _context.KhachHangs
				.FirstOrDefaultAsync(x => x.MaKH == maKH.Value);

			if (khachHang == null)
			{
				return RedirectToAction("Login", "Account");
			}

			return View(khachHang);
		}

		// 2. Hiển thị lịch sử tất cả các đơn hàng đã đặt
		public async Task<IActionResult> LichSuDonHang()
		{
			var maKH = HttpContext.Session.GetInt32("MaKH");

			if (maKH == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var donHangs = await _context.DonHangs
				.Where(x => x.MaKH == maKH.Value)
				.OrderByDescending(x => x.ThoiGianDat)
				.ToListAsync();

			return View(donHangs);
		}

		// 3. Xem chi tiết các món ăn trong một đơn hàng cụ thể
		public async Task<IActionResult> ChiTietDonHang(int maDonHang)
		{
			var maKH = HttpContext.Session.GetInt32("MaKH");

			if (maKH == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var donHang = await _context.DonHangs
				.FirstOrDefaultAsync(x => x.MaDonHang == maDonHang && x.MaKH == maKH.Value);

			if (donHang == null)
			{
				return RedirectToAction("LichSuDonHang");
			}

			var chiTiet = await _context.ChiTietDonHangs
				.Include(x => x.MonAn)
				.Where(x => x.MaDonHang == maDonHang)
				.ToListAsync();

			ViewBag.DonHang = donHang;

			return View(chiTiet);
		}

		// 4. Theo dõi tiến trình các đơn hàng đang xử lý (Real-time Timeline)
		public async Task<IActionResult> TrangThaiDonHang()
		{
			var maKH = HttpContext.Session.GetInt32("MaKH");

			if (maKH == null)
			{
				return RedirectToAction("Login", "Account");
			}

			// ĐÃ CẬP NHẬT: Cho phép lọc cả trạng thái "Hoàn thành" 
			// giúp đơn hàng không bị ẩn đi khi chuyển giao sang bước cuối
			var donHangsDangXuLy = await _context.DonHangs
				.Where(x => x.MaKH == maKH.Value
					&& (x.TrangThai == "Chờ xác nhận"
						|| x.TrangThai == "Đang chế biến"
						|| x.TrangThai == "Đã phục vụ"
						|| x.TrangThai == "Hoàn thành"))
				.OrderByDescending(x => x.ThoiGianDat)
				.ToListAsync();

			return View(donHangsDangXuLy);
		}
	}
}