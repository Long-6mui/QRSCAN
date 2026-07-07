using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRSCAN.Migrations
{
    /// <inheritdoc />
    public partial class AddThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cột PhuongThucThanhToan đã tồn tại sẵn trong DB (do bản khác đã tạo trước đó)
            // nên bỏ qua, không AddColumn lại nữa để tránh lỗi "Duplicate column name"
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không làm gì, vì Up() không tạo gì để phải rollback
        }
    }
}