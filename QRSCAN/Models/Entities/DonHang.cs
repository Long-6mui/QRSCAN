using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class DonHang
    {
        [Key]
        public int MaDH { get; set; }

        public int? MaVoucher { get; set; }

        public int MaPhien { get; set; }

        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        public decimal TongTien { get; set; }

        public string TrangThai { get; set; } = "ChoXacNhan";

        public decimal SoTienGiam { get; set; }

        [ForeignKey("MaVoucher")]
        public Voucher? Voucher { get; set; }

        [ForeignKey("MaPhien")]
        public PhienGoiMon? PhienGoiMon { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}