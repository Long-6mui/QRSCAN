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
        public DbSet<PhienGoiMon> PhienGoiMons { get; set; }

        public DbSet<KhachHang> KhachHangs { get; set; }

        public DbSet<DanhMucMon> DanhMucMons { get; set; }
        public DbSet<MonAn> MonAns { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BanAn>()
                .HasKey(x => x.MaBan);

            modelBuilder.Entity<PhienGoiMon>()
                .HasKey(x => x.MaPhien);

            modelBuilder.Entity<PhienGoiMon>()
                .HasOne(x => x.BanAn)
                .WithMany(x => x.PhienGoiMons)
                .HasForeignKey(x => x.MaBan);
            modelBuilder.Entity<KhachHang>().HasKey(x => x.MaKH);

            modelBuilder.Entity<KhachHang>().ToTable("KhachHang");

            modelBuilder.Entity<DanhMucMon>().ToTable("DanhMucMon");
            modelBuilder.Entity<MonAn>().ToTable("MonAn");

            modelBuilder.Entity<MonAn>()
                .HasOne(x => x.DanhMucMon)
                .WithMany(x => x.MonAns)
                .HasForeignKey(x => x.MaDanhMuc);

            modelBuilder.Entity<DonHang>().ToTable("DonHang");
            modelBuilder.Entity<ChiTietDonHang>().ToTable("ChiTietDonHang");

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.DonHang)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.MaDonHang);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.MonAn)
                .WithMany()
                .HasForeignKey(x => x.MaMon);

            modelBuilder.Entity<Voucher>().ToTable("Voucher");

            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<NhanVien>().ToTable("NhanVien");

            modelBuilder.Entity<NhanVien>()
                .HasOne(x => x.VaiTro)
                .WithMany(x => x.NhanViens)
                .HasForeignKey(x => x.MaVT);
        }
    }
}