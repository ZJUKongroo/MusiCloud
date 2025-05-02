using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddisExisttoMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "Metadatas");

            migrationBuilder.AddColumn<bool>(
                name: "IsExisted",
                table: "Metadatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "CoverPath",
                table: "Albums",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExisted",
                table: "Metadatas");

            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "Metadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CoverPath",
                table: "Albums",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
