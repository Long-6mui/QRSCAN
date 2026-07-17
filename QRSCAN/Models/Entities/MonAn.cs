using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class MonAn
    {
        [Key]
        public int MaMon { get; set; }

        public int MaDM { get; set; }

        public string TenMon { get; set; } = string.Empty;

        public string? HinhAnh { get; set; }

        public decimal DonGia { get; set; }

        public string? MoTa { get; set; }

        public string TrangThai { get; set; } = "DangBan";

        public int SoLuong { get; set; }

        [ForeignKey("MaDM")]
        public DanhMucMon? DanhMucMon { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}