using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVote.Migrations
{
    /// <inheritdoc />
    public partial class AddResults4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "finish",
                table: "Competitions",
                newName: "Finish");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Finish",
                table: "Competitions",
                newName: "finish");
        }
    }
}
