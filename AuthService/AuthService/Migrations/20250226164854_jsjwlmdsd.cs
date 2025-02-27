using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class jsjwlmdsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slot4Duration",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot4Price",
                table: "CourtSports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Slot4Duration",
                table: "CourtSports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Slot4Price",
                table: "CourtSports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
