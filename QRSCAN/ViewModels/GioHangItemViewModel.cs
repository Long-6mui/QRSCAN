namespace QRSCAN.ViewModels
{
    public class GioHangItemViewModel
    {
        public int MaMon { get; set; }

        public string TenMon { get; set; } = string.Empty;

        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        public string? HinhAnh { get; set; }

        public decimal ThanhTien
        {
            get
            {
                return DonGia * SoLuong;
            }
        }
    }
}