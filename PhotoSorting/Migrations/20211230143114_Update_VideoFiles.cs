using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoSorting.Migrations
{
    public partial class Update_VideoFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoFile_DoubleSets_DoubleSetId",
                table: "VideoFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoFile",
                table: "VideoFile");

            migrationBuilder.RenameTable(
                name: "VideoFile",
                newName: "VideoFiles");

            migrationBuilder.RenameIndex(
                name: "IX_VideoFile_DoubleSetId",
                table: "VideoFiles",
                newName: "IX_VideoFiles_DoubleSetId");

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "VideoFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoFiles",
                table: "VideoFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoFiles_DoubleSets_DoubleSetId",
                table: "VideoFiles",
                column: "DoubleSetId",
                principalTable: "DoubleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoFiles_DoubleSets_DoubleSetId",
                table: "VideoFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoFiles",
                table: "VideoFiles");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "VideoFiles");

            migrationBuilder.RenameTable(
                name: "VideoFiles",
                newName: "VideoFile");

            migrationBuilder.RenameIndex(
                name: "IX_VideoFiles_DoubleSetId",
                table: "VideoFile",
                newName: "IX_VideoFile_DoubleSetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoFile",
                table: "VideoFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoFile_DoubleSets_DoubleSetId",
                table: "VideoFile",
                column: "DoubleSetId",
                principalTable: "DoubleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
