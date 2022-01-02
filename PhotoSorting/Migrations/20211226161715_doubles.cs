using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoSorting.Migrations
{
    public partial class doubles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DoubleSetId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ToDelete",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DoubleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preferred = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoubleSets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_DoubleSetId",
                table: "Photos",
                column: "DoubleSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_DoubleSets_DoubleSetId",
                table: "Photos",
                column: "DoubleSetId",
                principalTable: "DoubleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_DoubleSets_DoubleSetId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "DoubleSets");

            migrationBuilder.DropIndex(
                name: "IX_Photos_DoubleSetId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "DoubleSetId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "ToDelete",
                table: "Photos");
        }
    }
}
