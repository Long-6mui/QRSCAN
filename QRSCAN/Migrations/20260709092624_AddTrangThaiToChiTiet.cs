using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRSCAN.Migrations
{
    /// <inheritdoc />
    public partial class AddTrangThaiToChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "ChiTietDonHang",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "ChiTietDonHang");
        }
    }
}
