using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class MusicArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Musics_MusicId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_MusicId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "MusicId",
                table: "Artists");

            migrationBuilder.CreateTable(
                name: "MusicArtists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MusicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArtistId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicArtists_Musics_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Musics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicArtists_ArtistId",
                table: "MusicArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicArtists_MusicId",
                table: "MusicArtists",
                column: "MusicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicArtists");

            migrationBuilder.AddColumn<Guid>(
                name: "MusicId",
                table: "Artists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_MusicId",
                table: "Artists",
                column: "MusicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Musics_MusicId",
                table: "Artists",
                column: "MusicId",
                principalTable: "Musics",
                principalColumn: "Id");
        }
    }
}
