using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class PhienGoiMon
    {
        [Key]
        public int MaPhien { get; set; }

        public int MaKH { get; set; }

        public int MaBan { get; set; }

        public DateTime BatDau { get; set; } = DateTime.Now;

        public DateTime? KetThuc { get; set; }

        public string TrangThai { get; set; } = "DangMo";

        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }

        [ForeignKey("MaBan")]
        public BanAn? BanAn { get; set; }

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}