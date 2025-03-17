using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeLeave.Data.Migrations
{
    /// <inheritdoc />
    public partial class approval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leaves_Profile_EmployeeId1",
                table: "leaves");

            migrationBuilder.DropIndex(
                name: "IX_leaves_EmployeeId1",
                table: "leaves");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "leaves");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_leaves_EmployeeId1",
                table: "leaves",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_leaves_Profile_EmployeeId1",
                table: "leaves",
                column: "EmployeeId1",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
