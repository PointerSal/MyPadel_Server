using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class iejkdpw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "CourtSports");

            migrationBuilder.CreateTable(
                name: "CourtSportsCustomDateTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtSportsId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosingTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtSportsCustomDateTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtSportsCustomDateTime_CourtSports_CourtSportsId",
                        column: x => x.CourtSportsId,
                        principalTable: "CourtSports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtSportsCustomDateTime_CourtSportsId",
                table: "CourtSportsCustomDateTime",
                column: "CourtSportsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtSportsCustomDateTime");

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "CourtSports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
