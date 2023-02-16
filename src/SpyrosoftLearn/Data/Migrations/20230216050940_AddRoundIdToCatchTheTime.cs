using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyrosoftLearn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundIdToCatchTheTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoundId",
                table: "CatchTheTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CatchTheTimes_RoundId",
                table: "CatchTheTimes",
                column: "RoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatchTheTimes_Rounds_RoundId",
                table: "CatchTheTimes",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatchTheTimes_Rounds_RoundId",
                table: "CatchTheTimes");

            migrationBuilder.DropIndex(
                name: "IX_CatchTheTimes_RoundId",
                table: "CatchTheTimes");

            migrationBuilder.DropColumn(
                name: "RoundId",
                table: "CatchTheTimes");
        }
    }
}
