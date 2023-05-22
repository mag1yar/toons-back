using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace toons.Migrations
{
    public partial class mangahasteams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MangaDtoTeamDto",
                columns: table => new
                {
                    MangaListId = table.Column<int>(type: "integer", nullable: false),
                    TeamsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaDtoTeamDto", x => new { x.MangaListId, x.TeamsId });
                    table.ForeignKey(
                        name: "FK_MangaDtoTeamDto_Manga_MangaListId",
                        column: x => x.MangaListId,
                        principalTable: "Manga",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaDtoTeamDto_Teams_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaDtoTeamDto_TeamsId",
                table: "MangaDtoTeamDto",
                column: "TeamsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaDtoTeamDto");
        }
    }
}
