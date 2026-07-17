using QRSCAN.Models.Entities;

namespace QRSCAN.ViewModels
{
    public class NhanVienDashboardViewModel
    {
        public List<DonHang> DonHangsCanXuLy { get; set; } = new List<DonHang>();

        public List<BanTrangThaiViewModel> BanAns { get; set; } = new List<BanTrangThaiViewModel>();
    }

    public class BanTrangThaiViewModel
    {
        public int MaBan { get; set; }

        public string TenBan { get; set; } = string.Empty;

        public string TrangThai { get; set; } = "Trong";

        public int? MaPhien { get; set; }

        public int? MaDH { get; set; }

        public decimal? TongTien { get; set; }
    }
}