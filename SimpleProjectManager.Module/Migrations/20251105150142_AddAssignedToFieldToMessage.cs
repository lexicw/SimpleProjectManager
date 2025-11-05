using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleProjectManager.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToFieldToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Messageid",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Messageid",
                table: "Employees",
                column: "Messageid");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Messages_Messageid",
                table: "Employees",
                column: "Messageid",
                principalTable: "Messages",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Messages_Messageid",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Messageid",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Messageid",
                table: "Employees");
        }
    }
}
