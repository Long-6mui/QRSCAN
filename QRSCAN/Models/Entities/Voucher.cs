using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class Voucher
    {
        [Key]
        public int MaVoucher { get; set; }

        public string TenVoucher { get; set; } = string.Empty;

        public string MaCode { get; set; } = string.Empty;

        public string LoaiGiamGia { get; set; } = string.Empty;

        public decimal GiaTriGiam { get; set; }

        public decimal DieuKien { get; set; }

        public DateTime BatDau { get; set; }

        public DateTime KetThuc { get; set; }

        public int SoLuong { get; set; }

        public string TrangThai { get; set; } = "HoatDong";

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}