using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetSentinel.Migrations
{
    /// <inheritdoc />
    public partial class BusinessRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_SecurityMeasures_PerformedAt_NotFuture",
                table: "SecurityMeasures",
                sql: "PerformedAt <= GETUTCDATE()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Incidents_CreatedAt_NotFuture",
                table: "Incidents",
                sql: "CreatedAt <= GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SecurityMeasures_PerformedAt_NotFuture",
                table: "SecurityMeasures");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Incidents_CreatedAt_NotFuture",
                table: "Incidents");
        }
    }
}
