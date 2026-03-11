using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetSentinel.Migrations
{
    /// <inheritdoc />
    public partial class PerformedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerformedById",
                table: "SecurityMeasures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityMeasures_PerformedById",
                table: "SecurityMeasures",
                column: "PerformedById");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityMeasures_Employees_PerformedById",
                table: "SecurityMeasures",
                column: "PerformedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SecurityMeasures_Employees_PerformedById",
                table: "SecurityMeasures");

            migrationBuilder.DropIndex(
                name: "IX_SecurityMeasures_PerformedById",
                table: "SecurityMeasures");

            migrationBuilder.DropColumn(
                name: "PerformedById",
                table: "SecurityMeasures");
        }
    }
}
