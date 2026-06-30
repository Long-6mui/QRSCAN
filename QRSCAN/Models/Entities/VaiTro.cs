using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class VaiTro
    {
        [Key]
        public int MaVT { get; set; }

        [Required]
        [StringLength(100)]
        public string TenVT { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public ICollection<NhanVien>? NhanViens { get; set; }
    }
}