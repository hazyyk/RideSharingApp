using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeAndLongitudeToYourEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Drivers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Drivers",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Drivers");
        }
    }
}
