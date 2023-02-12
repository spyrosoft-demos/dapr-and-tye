using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyrosoftLearn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPointsToCatchTheTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPoints",
                table: "CatchTheTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPoints",
                table: "CatchTheTimes");
        }
    }
}
