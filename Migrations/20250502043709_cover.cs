using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class Cover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "Metadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "Metadatas");
        }
    }
}
