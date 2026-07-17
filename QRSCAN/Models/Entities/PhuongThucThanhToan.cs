using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class PhuongThucThanhToan
    {
        [Key]
        public int MaPT { get; set; }

        public string TenPT { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public string TrangThai { get; set; } = "HoatDong";

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}