using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSignupApi.Migrations
{
    /// <inheritdoc />
    public partial class CompletedRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserOwnerEventRelations_EventId",
                table: "UserOwnerEventRelations");

            migrationBuilder.DropIndex(
                name: "IX_UserOwnerEventRelations_UserId",
                table: "UserOwnerEventRelations");

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnerEventRelations_EventId",
                table: "UserOwnerEventRelations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnerEventRelations_UserId",
                table: "UserOwnerEventRelations",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_GenreId",
                table: "Events",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventGenreLookup_GenreId",
                table: "Events",
                column: "GenreId",
                principalTable: "EventGenreLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventGenreLookup_GenreId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_UserOwnerEventRelations_EventId",
                table: "UserOwnerEventRelations");

            migrationBuilder.DropIndex(
                name: "IX_UserOwnerEventRelations_UserId",
                table: "UserOwnerEventRelations");

            migrationBuilder.DropIndex(
                name: "IX_Events_GenreId",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnerEventRelations_EventId",
                table: "UserOwnerEventRelations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnerEventRelations_UserId",
                table: "UserOwnerEventRelations",
                column: "UserId");
        }
    }
}
