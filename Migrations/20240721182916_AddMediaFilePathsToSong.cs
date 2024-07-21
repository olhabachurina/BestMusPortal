using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestMusPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFilePathsToSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicFilePath",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoFilePath",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicFilePath",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "VideoFilePath",
                table: "Songs");
        }
    }
}
