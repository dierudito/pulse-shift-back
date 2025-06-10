using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dm.PulseShift.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CardCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CardLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AssociatedStartTimeEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssociatedEndTimeEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPeriods_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityPeriods_TimeEntries_AssociatedEndTimeEntryId",
                        column: x => x.AssociatedEndTimeEntryId,
                        principalTable: "TimeEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityPeriods_TimeEntries_AssociatedStartTimeEntryId",
                        column: x => x.AssociatedStartTimeEntryId,
                        principalTable: "TimeEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CardCode",
                table: "Activities",
                column: "CardCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPeriods_ActivityId",
                table: "ActivityPeriods",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPeriods_AssociatedEndTimeEntryId",
                table: "ActivityPeriods",
                column: "AssociatedEndTimeEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPeriods_AssociatedStartTimeEntryId",
                table: "ActivityPeriods",
                column: "AssociatedStartTimeEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityPeriods");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
