using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestMusPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddMoodToSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "Songs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mood",
                table: "Songs");
        }
    }
}
