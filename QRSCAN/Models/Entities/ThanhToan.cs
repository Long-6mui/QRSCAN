using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        public int MaDonHang { get; set; }

        [StringLength(20)]
        public string PhuongThuc { get; set; } = "TienMat";

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ChoThanhToan";

        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        public DonHang? DonHang { get; set; }
    }
}
