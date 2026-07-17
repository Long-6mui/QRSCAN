using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }

        public int MaDH { get; set; }

        public int? MaNV { get; set; }

        public int MaPT { get; set; }

        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        public decimal TongTien { get; set; }

        public string TrangThai { get; set; } = "ChuaThanhToan";

        public decimal SoTienGiam { get; set; }

        [ForeignKey("MaDH")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }

        [ForeignKey("MaPT")]
        public PhuongThucThanhToan? PhuongThucThanhToan { get; set; }
    }
}