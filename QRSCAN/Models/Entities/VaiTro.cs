using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class VaiTro
    {
        [Key]
        public int MaVT { get; set; }

        public string TenVT { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
    }
}