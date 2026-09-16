using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessBuzz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BusinessTypes",
                columns: new[] { "BusinessTypeId", "TypeName" },
                values: new object[,]
                {
                    { 1, "Sole Proprietorship" },
                    { 2, "Private Company" },
                    { 3, "Partnership" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "DocumentTypeId", "IsRequired", "TypeName" },
                values: new object[,]
                {
                    { 1, true, "Identity Document" },
                    { 2, true, "Proof of Residence" },
                    { 3, false, "Business Registration Document" },
                    { 4, false, "Tax Document" }
                });

            migrationBuilder.InsertData(
                table: "JourneyStages",
                columns: new[] { "JourneyStageId", "InstitutionName", "IsRequired", "StageDescription", "StageName", "StageOrder", "StageType" },
                values: new object[,]
                {
                    { 1, "Business Buzz", true, "Create, review and confirm the entrepreneur's business profile.", "Business Profile", 1, "Profile" },
                    { 2, "CIPC", true, "Prepare the information and documents required for business registration and guide the entrepreneur to the official CIPC process.", "CIPC Registration", 2, "Institutional" },
                    { 3, "SARS", false, "Prepare and track relevant tax and compliance requirements based on the entrepreneur's business circumstances.", "SARS Compliance", 3, "Compliance" },
                    { 4, "UIF", false, "Prepare and track UIF-related requirements when applicable, such as when the business employs people.", "UIF Compliance", 4, "Compliance" },
                    { 5, "Business Buzz", true, "Review outstanding preparation tasks and determine whether the business is ready for the next stage of its journey.", "Business Readiness", 5, "Readiness" },
                    { 6, "Business Buzz", false, "Prepare the business for an optional funding assessment and organise relevant information and documents.", "Funding Readiness", 6, "Funding" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "User who creates and manages a business journey.", "Entrepreneur" },
                    { 2, "User who reviews applications and supporting information.", "Approver" },
                    { 3, "User who reviews funding enquiries and performs simulated funding assessments.", "Bank User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BusinessTypes",
                keyColumn: "BusinessTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BusinessTypes",
                keyColumn: "BusinessTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BusinessTypes",
                keyColumn: "BusinessTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "DocumentTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "DocumentTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "DocumentTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "DocumentTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "JourneyStages",
                keyColumn: "JourneyStageId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "RoleId",
                keyValue: 3);
        }
    }
}
