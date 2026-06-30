using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class MonAn
    {
        [Key]
        public int MaMon { get; set; }

        [Required]
        [StringLength(150)]
        public string TenMon { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "DangBan";

        public int MaDanhMuc { get; set; }

        [ForeignKey("MaDanhMuc")]
        public DanhMucMon? DanhMucMon { get; set; }
    }
}