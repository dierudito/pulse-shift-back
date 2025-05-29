using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dm.PulseShift.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class NonWorkingDaysToDaysOffMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NonWorkingDays",
                table: "NonWorkingDays");

            migrationBuilder.RenameTable(
                name: "NonWorkingDays",
                newName: "DaysOff");

            migrationBuilder.RenameIndex(
                name: "IX_NonWorkingDays_Date",
                table: "DaysOff",
                newName: "IX_DaysOff_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DaysOff",
                table: "DaysOff",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DaysOff",
                table: "DaysOff");

            migrationBuilder.RenameTable(
                name: "DaysOff",
                newName: "NonWorkingDays");

            migrationBuilder.RenameIndex(
                name: "IX_DaysOff_Date",
                table: "NonWorkingDays",
                newName: "IX_NonWorkingDays_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NonWorkingDays",
                table: "NonWorkingDays",
                column: "Id");
        }
    }
}
