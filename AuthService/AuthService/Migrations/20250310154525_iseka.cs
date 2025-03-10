using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class iseka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Municipality",
                table: "MembershipUsers",
                newName: "TaxCode");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "MembershipUsers",
                newName: "ResidentialAddress");

            migrationBuilder.AddColumn<string>(
                name: "Citizenship",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MunicipalityOfBirth",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MunicipalityOfResidence",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProvinceOfBirth",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProvinceOfResidence",
                table: "MembershipUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Citizenship",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "MunicipalityOfBirth",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "MunicipalityOfResidence",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "ProvinceOfBirth",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "ProvinceOfResidence",
                table: "MembershipUsers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "TaxCode",
                table: "MembershipUsers",
                newName: "Municipality");

            migrationBuilder.RenameColumn(
                name: "ResidentialAddress",
                table: "MembershipUsers",
                newName: "Address");
        }
    }
}
