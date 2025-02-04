using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSignupApi.Migrations
{
    /// <inheritdoc />
    public partial class LatLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Lat",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Long",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "Events");
        }
    }
}
