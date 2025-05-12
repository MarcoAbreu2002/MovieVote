using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterPath",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Movies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterPath",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
