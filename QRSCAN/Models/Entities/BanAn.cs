using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        [Required]
        [StringLength(100)]
        public string TenBan { get; set; } = string.Empty;

        public int SoChoNgoi { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = string.Empty;

        [StringLength(500)]
        public string NoiDungQR { get; set; } = string.Empty;

        public ICollection<PhienGoiMon>? PhienGoiMons { get; set; }
    }
}