using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaCT_DH { get; set; }

        public int MaDH { get; set; }

        public int MaMon { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public string? GhiChu { get; set; }

        public string TrangThai { get; set; } = "ChoCheBien";

        public decimal ThanhTien { get; set; }

        [ForeignKey("MaDH")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaMon")]
        public MonAn? MonAn { get; set; }
    }
}