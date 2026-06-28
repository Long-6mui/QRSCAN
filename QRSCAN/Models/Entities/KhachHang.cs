using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20)]
        public string SDT { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string LoaiKhach { get; set; } = "Thuong";

        public int DiemTichLuy { get; set; } = 0;

        [StringLength(50)]
        public string TrangThai { get; set; } = "HoatDong";

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100)]
        public string MatKhau { get; set; } = string.Empty;
    }
}