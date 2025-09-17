using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCeima.Migrations
{
    /// <inheritdoc />
    public partial class Addphoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserImg",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImg",
                table: "AspNetUsers");
        }
    }
}
