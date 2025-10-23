using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleProjectManager.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageBody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageBody",
                table: "Messages",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageBody",
                table: "Messages");
        }
    }
}
