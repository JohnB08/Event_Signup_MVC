using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSignupApi.Migrations
{
    /// <inheritdoc />
    public partial class AlteredRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAdminEventRelations");

            migrationBuilder.DropTable(
                name: "UserOwnerEventRelations");

            migrationBuilder.DropTable(
                name: "UserSignupEventRelations");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    AdminEventsEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminsUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => new { x.AdminEventsEventId, x.AdminsUserId });
                    table.ForeignKey(
                        name: "FK_EventUser_Events_AdminEventsEventId",
                        column: x => x.AdminEventsEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser_Users_AdminsUserId",
                        column: x => x.AdminsUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUser1",
                columns: table => new
                {
                    SignUpEventsEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    SignUpsUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser1", x => new { x.SignUpEventsEventId, x.SignUpsUserId });
                    table.ForeignKey(
                        name: "FK_EventUser1_Events_SignUpEventsEventId",
                        column: x => x.SignUpEventsEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser1_Users_SignUpsUserId",
                        column: x => x.SignUpsUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_AdminsUserId",
                table: "EventUser",
                column: "AdminsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser1_SignUpsUserId",
                table: "EventUser1",
                column: "SignUpsUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "EventUser1");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "UserAdminEventRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdminEventRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAdminEventRelations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAdminEventRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOwnerEventRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOwnerEventRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOwnerEventRelations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOwnerEventRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSignupEventRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSignupEventRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSignupEventRelations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSignupEventRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAdminEventRelations_EventId",
                table: "UserAdminEventRelations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdminEventRelations_UserId",
                table: "UserAdminEventRelations",
                column: "UserId");

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
                name: "IX_UserSignupEventRelations_EventId",
                table: "UserSignupEventRelations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSignupEventRelations_UserId",
                table: "UserSignupEventRelations",
                column: "UserId");
        }
    }
}
