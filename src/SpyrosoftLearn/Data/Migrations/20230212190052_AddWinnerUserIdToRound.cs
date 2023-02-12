using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyrosoftLearn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerUserIdToRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WinnerUserId",
                table: "Rounds",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerUserId",
                table: "Rounds");
        }
    }
}
