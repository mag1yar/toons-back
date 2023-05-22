using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace toons.Migrations
{
    public partial class temp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMangas_Manga_MangaId",
                table: "TeamMangas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMangas",
                table: "TeamMangas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manga",
                table: "Manga");

            migrationBuilder.RenameTable(
                name: "Manga",
                newName: "Mangas");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TeamMangas",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMangas",
                table: "TeamMangas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mangas",
                table: "Mangas",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMangas_TeamId",
                table: "TeamMangas",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMangas_Mangas_MangaId",
                table: "TeamMangas",
                column: "MangaId",
                principalTable: "Mangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMangas_Mangas_MangaId",
                table: "TeamMangas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMangas",
                table: "TeamMangas");

            migrationBuilder.DropIndex(
                name: "IX_TeamMangas_TeamId",
                table: "TeamMangas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mangas",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TeamMangas");

            migrationBuilder.RenameTable(
                name: "Mangas",
                newName: "Manga");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMangas",
                table: "TeamMangas",
                columns: new[] { "TeamId", "MangaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manga",
                table: "Manga",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMangas_Manga_MangaId",
                table: "TeamMangas",
                column: "MangaId",
                principalTable: "Manga",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
