using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVote.Migrations
{
    /// <inheritdoc />
    public partial class AddResults3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "finish",
                table: "Competitions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "finish",
                table: "Competitions");
        }
    }
}
