using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetSentinel.Migrations
{
    /// <inheritdoc />
    public partial class NaturalKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeNo",
                table: "Employees",
                newName: "EmployeeNumber");

            migrationBuilder.AddColumn<string>(
                name: "SecurityMeasureNumber",
                table: "SecurityMeasures",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IncidentNumber",
                table: "Incidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityMeasures_SecurityMeasureNumber",
                table: "SecurityMeasures",
                column: "SecurityMeasureNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentNumber",
                table: "Incidents",
                column: "IncidentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SerialNumber",
                table: "Devices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SecurityMeasures_SecurityMeasureNumber",
                table: "SecurityMeasures");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_IncidentNumber",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Devices_SerialNumber",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "SecurityMeasureNumber",
                table: "SecurityMeasures");

            migrationBuilder.DropColumn(
                name: "IncidentNumber",
                table: "Incidents");

            migrationBuilder.RenameColumn(
                name: "EmployeeNumber",
                table: "Employees",
                newName: "EmployeeNo");
        }
    }
}
