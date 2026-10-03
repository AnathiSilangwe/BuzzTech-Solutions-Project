using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessBuzz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessInformationDocumentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "DocumentTypeId", "IsRequired", "TypeName" },
                values: new object[] { 5, false, "Business Information Document" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "DocumentTypeId",
                keyValue: 5);
        }
    }
}
