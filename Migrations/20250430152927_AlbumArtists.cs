using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class AlbumArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "HashValue",
                table: "Metadatas");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Metadatas");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Musics",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Albums",
                newName: "Title");

            migrationBuilder.CreateTable(
                name: "AlbumArtists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlbumId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArtistId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumArtists_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtists_AlbumId",
                table: "AlbumArtists",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtists_ArtistId",
                table: "AlbumArtists",
                column: "ArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumArtists");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Musics",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Albums",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "HashValue",
                table: "Metadatas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                table: "Metadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "AlbumId",
                table: "Artists",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists",
                column: "AlbumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
