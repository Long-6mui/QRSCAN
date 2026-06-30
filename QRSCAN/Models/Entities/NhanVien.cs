using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class NhanVien
    {
        [Key]
        public int MaNV { get; set; }

        public int MaVT { get; set; }

        [Required]
        [StringLength(150)]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(20)]
        public string? SDT { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string MatKhau { get; set; } = string.Empty;

        [StringLength(50)]
        public string TrangThai { get; set; } = "HoatDong";

        [ForeignKey("MaVT")]
        public VaiTro? VaiTro { get; set; }
    }
}