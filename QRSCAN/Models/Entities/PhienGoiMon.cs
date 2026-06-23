using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class PhienGoiMon
    {
        [Key]
        public int MaPhien { get; set; }

        public int MaBan { get; set; }

        [ForeignKey("MaBan")]
        public BanAn? BanAn { get; set; }

        public DateTime BatDau { get; set; }

        public DateTime? KetThuc { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = string.Empty;
    }
}