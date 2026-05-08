using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fhir.Terminology.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTerminology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BindingRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StructureDefinitionUrl = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    ElementPath = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    ValueSetCanonical = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    Strength = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindingRegistry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminologyResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResourceType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    LogicalId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RawJson = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologyResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpstreamServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SystemUriPrefix = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    BaseUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    AuthorizationHeader = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpstreamServers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_LogicalId",
                table: "TerminologyResources",
                column: "LogicalId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_Name",
                table: "TerminologyResources",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_ResourceType",
                table: "TerminologyResources",
                column: "ResourceType");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_ResourceType_LogicalId",
                table: "TerminologyResources",
                columns: new[] { "ResourceType", "LogicalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_Status",
                table: "TerminologyResources",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_Title",
                table: "TerminologyResources",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_Url",
                table: "TerminologyResources",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyResources_Version",
                table: "TerminologyResources",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BindingRegistry");

            migrationBuilder.DropTable(
                name: "TerminologyResources");

            migrationBuilder.DropTable(
                name: "UpstreamServers");
        }
    }
}
