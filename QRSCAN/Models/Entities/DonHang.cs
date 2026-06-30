using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRSCAN.Models.Entities
{
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        public int MaKH { get; set; }

        public int? MaBan { get; set; }

        public DateTime ThoiGianDat { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string TrangThai { get; set; } = "ChoXacNhan";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public int? MaVoucher { get; set; }

        [StringLength(50)]
        public string? MaVoucherCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongThanhToan { get; set; }

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }
}