using Microsoft.EntityFrameworkCore;
using QRSCAN.Models.Entities;

namespace QRSCAN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BanAn> BanAns { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<PhienGoiMon> PhienGoiMons { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<DanhMucMon> DanhMucMons { get; set; }
        public DbSet<MonAn> MonAns { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table names
            modelBuilder.Entity<BanAn>().ToTable("BanAn");
            modelBuilder.Entity<KhachHang>().ToTable("KhachHang");
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<NhanVien>().ToTable("NhanVien");
            modelBuilder.Entity<PhienGoiMon>().ToTable("PhienGoiMon");
            modelBuilder.Entity<Voucher>().ToTable("Voucher");
            modelBuilder.Entity<DonHang>().ToTable("DonHang");
            modelBuilder.Entity<PhuongThucThanhToan>().ToTable("PhuongThucThanhToan");
            modelBuilder.Entity<HoaDon>().ToTable("HoaDon");
            modelBuilder.Entity<DanhMucMon>().ToTable("DanhMucMon");
            modelBuilder.Entity<MonAn>().ToTable("MonAn");
            modelBuilder.Entity<ChiTietDonHang>().ToTable("ChiTietDonHang");

            // Primary keys
            modelBuilder.Entity<BanAn>().HasKey(x => x.MaBan);
            modelBuilder.Entity<KhachHang>().HasKey(x => x.MaKH);
            modelBuilder.Entity<VaiTro>().HasKey(x => x.MaVT);
            modelBuilder.Entity<NhanVien>().HasKey(x => x.MaNV);
            modelBuilder.Entity<PhienGoiMon>().HasKey(x => x.MaPhien);
            modelBuilder.Entity<Voucher>().HasKey(x => x.MaVoucher);
            modelBuilder.Entity<DonHang>().HasKey(x => x.MaDH);
            modelBuilder.Entity<PhuongThucThanhToan>().HasKey(x => x.MaPT);
            modelBuilder.Entity<HoaDon>().HasKey(x => x.MaHD);
            modelBuilder.Entity<DanhMucMon>().HasKey(x => x.MaDM);
            modelBuilder.Entity<MonAn>().HasKey(x => x.MaMon);
            modelBuilder.Entity<ChiTietDonHang>().HasKey(x => x.MaCT_DH);

            // Decimal config
            modelBuilder.Entity<Voucher>()
                .Property(x => x.GiaTriGiam)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Voucher>()
                .Property(x => x.DieuKien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DonHang>()
                .Property(x => x.TongTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DonHang>()
                .Property(x => x.SoTienGiam)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.TongTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.SoTienGiam)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonAn>()
                .Property(x => x.DonGia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ChiTietDonHang>()
                .Property(x => x.DonGia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ChiTietDonHang>()
                .Property(x => x.ThanhTien)
                .HasColumnType("decimal(18,2)");

            // Relationships
            modelBuilder.Entity<NhanVien>()
                .HasOne(x => x.VaiTro)
                .WithMany(x => x.NhanViens)
                .HasForeignKey(x => x.MaVT);

            modelBuilder.Entity<PhienGoiMon>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.PhienGoiMons)
                .HasForeignKey(x => x.MaKH);

            modelBuilder.Entity<PhienGoiMon>()
                .HasOne(x => x.BanAn)
                .WithMany(x => x.PhienGoiMons)
                .HasForeignKey(x => x.MaBan);

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.Voucher)
                .WithMany(x => x.DonHangs)
                .HasForeignKey(x => x.MaVoucher)
                .IsRequired(false);

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.PhienGoiMon)
                .WithMany(x => x.DonHangs)
                .HasForeignKey(x => x.MaPhien);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.DonHang)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaDH);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaNV)
                .IsRequired(false);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.PhuongThucThanhToan)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaPT);

            modelBuilder.Entity<MonAn>()
                .HasOne(x => x.DanhMucMon)
                .WithMany(x => x.MonAns)
                .HasForeignKey(x => x.MaDM);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.DonHang)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.MaDH);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.MonAn)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.MaMon);
        }
    }
}