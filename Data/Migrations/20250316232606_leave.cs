using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeLeave.Data.Migrations
{
    /// <inheritdoc />
    public partial class leave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
