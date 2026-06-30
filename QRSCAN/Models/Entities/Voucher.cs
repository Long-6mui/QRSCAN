using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class Voucher
    {
        [Key]
        public int MaVoucher { get; set; }

        [Required]
        [StringLength(50)]
        public string MaCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string TenVoucher { get; set; } = string.Empty;

        public int PhanTramGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamToiDa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonToiThieu { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "HoatDong";
    }
}