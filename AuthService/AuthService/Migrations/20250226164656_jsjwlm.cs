using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class jsjwlm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CourtFields",
                table: "CourtSports",
                newName: "Slot4Duration");

            migrationBuilder.AddColumn<bool>(
                name: "CanBeBooked",
                table: "CourtSports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FieldCapacity",
                table: "CourtSports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FieldName",
                table: "CourtSports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "CourtSports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "CourtSports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Slot1Duration",
                table: "CourtSports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Slot1Price",
                table: "CourtSports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Slot2Duration",
                table: "CourtSports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Slot2Price",
                table: "CourtSports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Slot3Duration",
                table: "CourtSports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Slot3Price",
                table: "CourtSports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Slot4Price",
                table: "CourtSports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TerrainType",
                table: "CourtSports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBeBooked",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "FieldCapacity",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "FieldName",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot1Duration",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot1Price",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot2Duration",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot2Price",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot3Duration",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot3Price",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "Slot4Price",
                table: "CourtSports");

            migrationBuilder.DropColumn(
                name: "TerrainType",
                table: "CourtSports");

            migrationBuilder.RenameColumn(
                name: "Slot4Duration",
                table: "CourtSports",
                newName: "CourtFields");
        }
    }
}
