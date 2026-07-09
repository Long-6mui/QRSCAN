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

		// Hàm kiểm tra quyền Chef (MaVT = 2)
		private bool IsChefLoggedIn()
		{
			var maVT = HttpContext.Session.GetInt32("MaVT");
			return maVT == 2;
		}

		// 1. Hiển thị danh sách đơn hàng cho màn hình nhà bếp
		public async Task<IActionResult> Index()
		{
			if (!IsChefLoggedIn())
			{
				return RedirectToAction("Login", "Account");
			}

			var danhSachDonHang = await _context.DonHangs
				.Include(x => x.ChiTietDonHangs)
					.ThenInclude(c => c.MonAn)
				.Where(x => x.TrangThai == "Chờ xác nhận"
							|| x.TrangThai == "Đang chế biến"
							|| x.TrangThai == "Đã phục vụ"
							|| x.TrangThai == "Hoàn thành")
				.OrderBy(x => x.ThoiGianDat)
				.ToListAsync();

			return View(danhSachDonHang);
		}

		// 2. Tiếp nhận toàn bộ đơn hàng (Chuyển trạng thái đơn sang "Đang chế biến")
		[HttpPost]
		public async Task<IActionResult> TiepNhanDonHang(int id)
		{
			if (!IsChefLoggedIn())
			{
				return RedirectToAction("Login", "Account");
			}

			var donHang = await _context.DonHangs
				.FirstOrDefaultAsync(x => x.MaDonHang == id);

			if (donHang != null)
			{
				donHang.TrangThai = "Đang chế biến";
				_context.DonHangs.Update(donHang);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		// 3. Xác nhận hoàn thành từng món ăn riêng lẻ
		[HttpPost]
		public async Task<IActionResult> HoanThanhMon(int id)
		{
			if (!IsChefLoggedIn())
			{
				return RedirectToAction("Login", "Account");
			}

			var chiTiet = await _context.ChiTietDonHangs
				.FirstOrDefaultAsync(x => x.MaChiTiet == id);

			if (chiTiet != null)
			{
				chiTiet.TrangThai = "Hoàn thành";
				_context.ChiTietDonHangs.Update(chiTiet);
				await _context.SaveChangesAsync();

				// Tự động chuyển đơn hàng sang "Đã phục vụ" khi tất cả các món trong đơn đã xong
				var maDonHang = chiTiet.MaDonHang;
				var tatCaMon = await _context.ChiTietDonHangs
					.Where(x => x.MaDonHang == maDonHang)
					.ToListAsync();

				if (tatCaMon.All(x => x.TrangThai == "Hoàn thành"))
				{
					var donHang = await _context.DonHangs
						.FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);

					if (donHang != null && donHang.TrangThai == "Đang chế biến")
					{
						donHang.TrangThai = "Đã phục vụ";
						_context.DonHangs.Update(donHang);
						await _context.SaveChangesAsync();
					}
				}
			}

			return RedirectToAction(nameof(Index));
		}

		// 4. Xác nhận hoàn tất toàn bộ đơn hàng (Chuyển sang "Hoàn thành")
		[HttpPost]
		public async Task<IActionResult> HoanThanhDonHang(int maDonHang)
		{
			if (!IsChefLoggedIn())
			{
				return RedirectToAction("Login", "Account");
			}

			var donHang = await _context.DonHangs
				.FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);

			if (donHang != null)
			{
				donHang.TrangThai = "Hoàn thành";
				_context.DonHangs.Update(donHang);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}
	}
}