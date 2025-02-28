using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class ieise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMarketing",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundRequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefundStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropColumn(
                name: "IsMarketing",
                table: "Users");
        }
    }
}
