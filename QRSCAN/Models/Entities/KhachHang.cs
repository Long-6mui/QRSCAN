using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        public string HoTen { get; set; } = string.Empty;

        public string? SDT { get; set; }

        public string? Email { get; set; }

        public string? LoaiKhach { get; set; }

        public int DiemTichLuy { get; set; } = 0;

        public string TrangThai { get; set; } = "HoatDong";

        public string TenDangNhap { get; set; } = string.Empty;

        public string MatKhau { get; set; } = string.Empty;

        public ICollection<PhienGoiMon> PhienGoiMons { get; set; } = new List<PhienGoiMon>();
    }
}