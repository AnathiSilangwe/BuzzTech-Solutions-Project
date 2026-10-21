using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BusinessDocument = BusinessBuzz.Core.Entities.Document;

namespace BusinessBuzz.Infrastructure.Services;

public class DigiDocsService : IDigiDocsService
{
    private readonly BusinessBuzzDbContext _context;
    

    public DigiDocsService(
        BusinessBuzzDbContext context
        )
    {
        _context = context;
        
    }

    public async Task<BusinessDocument?> GenerateBusinessInformationDocumentAsync(
        int businessProfileId,
        string idNumber)
    {
        var businessProfile = await _context.BusinessProfiles
            .Include(b => b.BusinessType)
            .FirstOrDefaultAsync(
                b => b.BusinessProfileId == businessProfileId);

        if (businessProfile == null)
        {
            return null;
        }

        var documentType = await _context.DocumentTypes
            .FirstOrDefaultAsync(
                d => d.TypeName == "Business Information Document");

        if (documentType == null)
        {
            return null;
        }

        // QuestPDF licensing configuration for this demonstration project.
        QuestPDF.Settings.License = LicenseType.Community;

        // Save generated files outside wwwroot.
        var documentsFolder = Path.GetFullPath(
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "App_Data",
            "GeneratedDocuments"));

        Directory.CreateDirectory(documentsFolder);

        var fileName =
            $"BusinessPlan_{businessProfile.BusinessProfileId}_{Guid.NewGuid():N}.pdf";

        var filePath = Path.Combine(documentsFolder, fileName);

        var businessName = businessProfile.BusinessName;
        var businessType = businessProfile.BusinessType.TypeName;
        var industry = businessProfile.Industry;
        var location = businessProfile.BusinessLocation;
        var generatedDate = DateTime.Now.ToString("dd MMMM yyyy");

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(45);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("BUSINESS BUZZ")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .PaddingTop(4)
                            .LineHorizontal(2)
                            .LineColor(Colors.Amber.Darken2);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .PaddingTop(45)
                            .AlignCenter()
                            .Text("BUSINESS PROFILE")
                            .FontSize(14)
                            .Bold();

                        column.Item()
                            .AlignCenter()
                            .Text(" & DEVELOPMENT PLAN")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .PaddingTop(25)
                            .AlignCenter()
                            .Text(businessName)
                            .FontSize(18)
                            .Bold();

                        column.Item()
                            .PaddingTop(15)
                            .AlignCenter()
                            .Text($"Business type: {businessType}");

                        column.Item()
                            .AlignCenter()
                            .Text($"Industry: {industry}");

                        column.Item()
                            .AlignCenter()
                            .Text($"Location: {location}");

                        column.Item()
                            .PaddingTop(20)
                            .AlignCenter()
                            .Text($"Prepared on: {generatedDate}");

                        column.Item()
                            .PaddingTop(35)
                            .AlignCenter()
                            .Text("Prepared using Business Buzz DigiDocs");

                        column.Item()
                            .PaddingTop(25)
                            .Text(
                                "DEMONSTRATION DOCUMENT — " +
                                "not an official government-issued document.")
                            .FontSize(9)
                            .Italic();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Business Buzz | Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(45);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("BUSINESS PROFILE & DEVELOPMENT PLAN")
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                page.Content()
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        AddSection(column, "1. Executive Summary",
                            $"This document introduces {businessName}, " +
                            $"a {businessType} business operating in the " +
                            $"{industry} sector in {location}. " +
                            "The business owner should complete this plan " +
                            "with their offering, target customers, objectives, " +
                            "and funding requirements.");

                        AddSection(column, "2. Business Description",
                            $"Business name: {businessName}\n" +
                            $"Business type: {businessType}\n" +
                            $"Industry: {industry}\n" +
                            $"Location: {location}\n\n" +
                            "Business activities, ownership details, and " +
                            "operating model: To be completed by the owner.");

                        AddSection(column, "3. Vision and Mission",
                            "Vision: To be defined by the business owner.\n\n" +
                            "Mission: Describe the customers served, the " +
                            "problem addressed, and how the business will " +
                            "deliver value.");

                        AddSection(column, "4. People and Leadership",
                            "List each owner or team member, their role, " +
                            "responsibilities, and business contact details. " +
                            "Complete this section with verified information.");

                        AddSection(column, "5. Core Values",
                            "Define the principles guiding the business, " +
                            "such as integrity, customer service, quality, " +
                            "accountability, and innovation, where applicable.");

                        AddSection(column, "6. Business Goals",
                            "Short term (0–12 months): To be completed.\n\n" +
                            "Medium term (1–3 years): To be completed.\n\n" +
                            "Long term (3+ years): To be completed.");

                        AddSection(column, "7. Objectives",
                            "For each objective, record the intended outcome, " +
                            "target date, resources required, and a measurable " +
                            "indicator of success.");

                        AddSection(column, "8. Market and Location Analysis",
                            "Describe the target market, customer needs, " +
                            "competitors, location suitability, and market " +
                            "opportunities. Research is required to validate " +
                            "these details.");

                        AddSection(column, "9. Products and Services",
                            "List the products or services offered, their " +
                            "benefits, pricing approach, and what distinguishes " +
                            "them from alternatives.");

                        AddSection(column, "10. Marketing and Sales Strategy",
                            "Describe the intended customers, promotional " +
                            "channels, sales process, pricing, and customer " +
                            "retention strategy.");

                        AddSection(column, "11. Operations Plan",
                            "Document daily activities, equipment, suppliers, " +
                            "staffing, premises, and any relevant licences " +
                            "or regulatory requirements.");

                        AddSection(column, "12. Financial Plan",
                            "Complete start-up costs, operating expenses, " +
                            "pricing, sales forecasts, cash-flow estimates, " +
                            "and funding requirements using realistic figures. " +
                            "No financial projections have been assumed here.");

                        AddSection(column, "13. Risk Management",
                            "Identify business risks, their likelihood and " +
                            "impact, and practical mitigation measures. " +
                            "Review risks regularly.");

                        AddSection(column, "14. Conclusion",
                            "This plan is a starting point for business " +
                            "development. The owner should review, complete, " +
                            "and update it as the business progresses.");

                        AddSection(column, "15. Sign-off",
                            "Business owner name: __________________________\n\n" +
                            "Signature: _____________________________________\n\n" +
                            $"Date: {generatedDate}\n\n" +
                            "This document is a planning aid and does not " +
                            "constitute regulatory approval or a funding offer.");
                    });
                
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Business Buzz | Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        }).GeneratePdf(filePath);

        var document = new BusinessDocument
        {
            BusinessProfileId = businessProfileId,
            DocumentTypeId = documentType.DocumentTypeId,
            FilePath = filePath,
            IsSystemGenerated = true,
            IsVerified = false,
            UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    private static void AddSection(
        ColumnDescriptor column,
        string heading,
        string content)
    {
        column.Item()
            .PaddingTop(8)
            .Text(heading)
            .FontSize(13)
            .Bold()
            .FontColor(Colors.Blue.Darken3);

        column.Item()
            .Text(content)
            .FontSize(10);
    }
}
