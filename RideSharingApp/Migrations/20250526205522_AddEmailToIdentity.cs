using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Customers");
        }
    }
}
