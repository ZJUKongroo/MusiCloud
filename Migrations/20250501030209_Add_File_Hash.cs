using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddFileHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileHash",
                table: "Metadatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "Metadatas");
        }
    }
}
