using System.ComponentModel.DataAnnotations;

namespace QRSCAN.Models.Entities
{
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        public string TenBan { get; set; } = string.Empty;

        public int SoChoNgoi { get; set; }

        public string TrangThai { get; set; } = "Trong";

        public string? NoiDungQR { get; set; }

        public ICollection<PhienGoiMon> PhienGoiMons { get; set; } = new List<PhienGoiMon>();
    }
}