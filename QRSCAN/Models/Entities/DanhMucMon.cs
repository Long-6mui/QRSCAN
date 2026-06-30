using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class DanhMucMon
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDanhMuc { get; set; } = string.Empty;

        public ICollection<MonAn>? MonAns { get; set; }
    }
}