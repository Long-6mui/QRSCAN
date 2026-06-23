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
        }
    }
}