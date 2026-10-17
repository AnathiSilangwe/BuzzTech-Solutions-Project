using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessBuzz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCipcKnowledgeArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "KnowledgeArticles",
                columns: new[] { "KnowledgeArticleId", "Content", "IsActive", "Keywords", "LastVerifiedAt", "SourceName", "SourceUrl", "Summary", "Title", "Topic" },
                values: new object[] { 1, "The Companies and Intellectual Property Commission (CIPC) is a South African government agency responsible for company registration and certain intellectual property services. Entrepreneurs establishing a private company generally use CIPC to register the company. Sole proprietors do not register a separate company with CIPC simply to operate as sole proprietors. The appropriate registration route depends on the legal structure of the business.", true, "CIPC, company registration, private company, sole proprietor, business registration", new DateTime(2026, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Companies and Intellectual Property Commission", "https://www.cipc.co.za/", "CIPC administers company registration and related intellectual property services in South Africa.", "What is CIPC?", "Business Registration" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeArticles",
                keyColumn: "KnowledgeArticleId",
                keyValue: 1);
        }
    }
}
