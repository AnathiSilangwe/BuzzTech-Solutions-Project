using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessBuzz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFundingAssessmentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BankUserId",
                table: "FundingAssessments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessedAt",
                table: "FundingAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FundingAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "FundingAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessedAt",
                table: "FundingAssessments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FundingAssessments");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "FundingAssessments");

            migrationBuilder.AlterColumn<int>(
                name: "BankUserId",
                table: "FundingAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
