using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class jajwDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FitMembershipFee",
                table: "PriceTbls");

            migrationBuilder.CreateTable(
                name: "FitMembershipTbls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FitMembershipFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitMembershipTbls", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FitMembershipTbls");

            migrationBuilder.AddColumn<decimal>(
                name: "FitMembershipFee",
                table: "PriceTbls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
