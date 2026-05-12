using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fhir.Terminology.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTerminologyPackageImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TerminologyPackageImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PackageVersion = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FhirVersion = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    ImportedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SourceUri = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    Sha256Hex = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    ResourceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologyPackageImports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyPackageImports_ImportedAtUtc",
                table: "TerminologyPackageImports",
                column: "ImportedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyPackageImports_PackageId_PackageVersion",
                table: "TerminologyPackageImports",
                columns: new[] { "PackageId", "PackageVersion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminologyPackageImports");
        }
    }
}
