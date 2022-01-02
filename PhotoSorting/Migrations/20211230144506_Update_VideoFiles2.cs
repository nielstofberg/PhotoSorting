using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoSorting.Migrations
{
    public partial class Update_VideoFiles2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "VideoFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "VideoFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
