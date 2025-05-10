using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddTracktoMusic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "Track",
                table: "Musics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Track",
                table: "Musics");
        }
    }
}
