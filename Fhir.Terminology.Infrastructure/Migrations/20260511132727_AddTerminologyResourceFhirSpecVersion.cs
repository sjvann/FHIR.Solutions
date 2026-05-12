using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fhir.Terminology.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTerminologyResourceFhirSpecVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TerminologyResources_ResourceType_LogicalId",
                table: "TerminologyResources");

            migrationBuilder.AddColumn<string>(
                name: "FhirSpecVersion",
                table: "TerminologyResources",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "5.0.0");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_FhirSpecVersion",
                table: "TerminologyResources",
                column: "FhirSpecVersion");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_ResourceType_LogicalId_FhirSpecVersion",
                table: "TerminologyResources",
                columns: new[] { "ResourceType", "LogicalId", "FhirSpecVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TerminologyResources_FhirSpecVersion",
                table: "TerminologyResources");

            migrationBuilder.DropIndex(
                name: "IX_TerminologyResources_ResourceType_LogicalId_FhirSpecVersion",
                table: "TerminologyResources");

            migrationBuilder.DropColumn(
                name: "FhirSpecVersion",
                table: "TerminologyResources");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_ResourceType_LogicalId",
                table: "TerminologyResources",
                columns: new[] { "ResourceType", "LogicalId" },
                unique: true);
        }
    }
}
