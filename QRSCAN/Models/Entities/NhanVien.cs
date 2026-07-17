using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class NhanVien
    {
        [Key]
        public int MaNV { get; set; }

        public int MaVT { get; set; }

        public string HoTen { get; set; } = string.Empty;

        public string? SDT { get; set; }

        public string? Email { get; set; }

        public string TenDangNhap { get; set; } = string.Empty;

        public string MatKhau { get; set; } = string.Empty;

        public string TrangThai { get; set; } = "HoatDong";

        [ForeignKey("MaVT")]
        public VaiTro? VaiTro { get; set; }

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}