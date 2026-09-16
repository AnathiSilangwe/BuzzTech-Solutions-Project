using BusinessBuzz.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Data;

public class BusinessBuzzDbContext : DbContext
{
    public BusinessBuzzDbContext(DbContextOptions<BusinessBuzzDbContext> options) : base(options)
    {
        
    }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<SystemUser> SystemUsers { get; set; }
    public DbSet<BusinessType> BusinessTypes { get; set; }
    public DbSet<BusinessProfile> BusinessProfiles { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<JourneyStage> JourneyStages { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ComplianceRecord> ComplianceRecords { get; set; }
    public DbSet<ReadinessTask> ReadinessTasks { get; set; }
    public DbSet<FundingAssessment> FundingAssessments { get; set; }
    public DbSet<ApprovalReview> ApprovalReviews { get; set; }
    public DbSet<InformationRequest> InformationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(r => r.RoleId);

        modelBuilder.Entity<SystemUser>()
            .HasKey(u => u.SystemUserId);

        modelBuilder.Entity<BusinessType>()
            .HasKey(t => t.BusinessTypeId);

        modelBuilder.Entity<BusinessProfile>()
            .HasKey(b => b.BusinessProfileId);

        modelBuilder.Entity<Notification>()
            .HasKey(n => n.NotificationId);

        modelBuilder.Entity<AuditLog>()
            .HasKey(a => a.AuditLogId);

        modelBuilder.Entity<DocumentType>()
            .HasKey(d => d.DocumentTypeId);

        modelBuilder.Entity<Document>()
            .HasKey(d => d.DocumentId);

        modelBuilder.Entity<JourneyStage>()
            .HasKey(j => j.JourneyStageId);

        modelBuilder.Entity<Application>()
            .HasKey(a => a.ApplicationId);

        modelBuilder.Entity<ComplianceRecord>()
            .HasKey(c => c.ComplianceRecordId);

        modelBuilder.Entity<ReadinessTask>()
            .HasKey(t => t.ReadinessTaskId);

        modelBuilder.Entity<FundingAssessment>()
            .HasKey(f => f.FundingAssessmentId);

        modelBuilder.Entity<ApprovalReview>()
            .HasKey(r => r.ApprovalReviewId);

        modelBuilder.Entity<InformationRequest>()
            .HasKey(i => i.InformationRequestId);

        //UserRole to SystemUser
        modelBuilder.Entity<SystemUser>()
            .HasOne(u => u.Role)
            .WithMany(r => r.SystemUsers)
            .HasForeignKey(u => u.RoleId);
        
        //BusinessType to BusinessProfile
        modelBuilder.Entity<BusinessProfile>()
            .HasOne(b => b.BusinessType)
            .WithMany(t => t.BusinessProfiles)
            .HasForeignKey(b => b.BusinessTypeId);
        //BusinessProfile to Document
        modelBuilder.Entity<Document>()
            .HasOne(d => d.BusinessProfile)
            .WithMany(b => b.Documents)
            .HasForeignKey(d => d.BusinessProfileId);
        // DocumentType → Document
        modelBuilder.Entity<Document>()
            .HasOne(d => d.DocumentType)
            .WithMany(t => t.Documents)
            .HasForeignKey(d => d.DocumentTypeId);

        // BusinessProfile → Application
        modelBuilder.Entity<Application>()
            .HasOne(a => a.BusinessProfile)
            .WithMany(b => b.Applications)
            .HasForeignKey(a => a.BusinessProfileId);

        // JourneyStage → Application
        modelBuilder.Entity<Application>()
            .HasOne(a => a.JourneyStage)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JourneyStageId);

        // Application → ApprovalReview
        modelBuilder.Entity<ApprovalReview>()
            .HasOne(r => r.Application)
            .WithMany(a => a.ApprovalReviews)
            .HasForeignKey(r => r.ApplicationId);

        // Approver → ApprovalReview
        modelBuilder.Entity<ApprovalReview>()
            .HasOne(r => r.Approver)
            .WithMany(u => u.ApprovalReviews)
            .HasForeignKey(r => r.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Application → InformationRequest
        modelBuilder.Entity<InformationRequest>()
            .HasOne(i => i.Application)
            .WithMany(a => a.InformationRequests)
            .HasForeignKey(i => i.ApplicationId);

        // Approver → InformationRequest
        modelBuilder.Entity<InformationRequest>()
            .HasOne(i => i.Approver)
            .WithMany()
            .HasForeignKey(i => i.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // BusinessProfile → ComplianceRecord
        modelBuilder.Entity<ComplianceRecord>()
            .HasOne(c => c.BusinessProfile)
            .WithMany(b => b.ComplianceRecords)
            .HasForeignKey(c => c.BusinessProfileId);

        // BusinessProfile → ReadinessTask
        modelBuilder.Entity<ReadinessTask>()
            .HasOne(t => t.BusinessProfile)
            .WithMany(b => b.ReadinessTasks)
            .HasForeignKey(t => t.BusinessProfileId);

        // BusinessProfile → FundingAssessment
        modelBuilder.Entity<FundingAssessment>()
            .HasOne(f => f.BusinessProfile)
            .WithMany(b => b.FundingAssessments)
            .HasForeignKey(f => f.BusinessProfileId);

        // BankUser → FundingAssessment
        modelBuilder.Entity<FundingAssessment>()
            .HasOne(f => f.BankUser)
            .WithMany(u => u.FundingAssessments)
            .HasForeignKey(f => f.BankUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // SystemUser → Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.SystemUser)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.SystemUserId);

        // SystemUser → AuditLog
        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.SystemUser)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.SystemUserId);

                // Seed User Roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                RoleId = 1,
                RoleName = "Entrepreneur",
                Description = "User who creates and manages a business journey."
            },
            new UserRole
            {
                RoleId = 2,
                RoleName = "Approver",
                Description = "User who reviews applications and supporting information."
            },
            new UserRole
            {
                RoleId = 3,
                RoleName = "Bank User",
                Description = "User who reviews funding enquiries and performs simulated funding assessments."
            }
        );

        // Seed Business Types
        modelBuilder.Entity<BusinessType>().HasData(
            new BusinessType
            {
                BusinessTypeId = 1,
                TypeName = "Sole Proprietorship"
            },
            new BusinessType
            {
                BusinessTypeId = 2,
                TypeName = "Private Company"
            },
            new BusinessType
            {
                BusinessTypeId = 3,
                TypeName = "Partnership"
            }
        );

        // Seed Document Types
        modelBuilder.Entity<DocumentType>().HasData(
            new DocumentType
            {
                DocumentTypeId = 1,
                TypeName = "Identity Document",
                IsRequired = true
            },
            new DocumentType
            {
                DocumentTypeId = 2,
                TypeName = "Proof of Residence",
                IsRequired = true
            },
            new DocumentType
            {
                DocumentTypeId = 3,
                TypeName = "Business Registration Document",
                IsRequired = false
            },
            new DocumentType
            {
                DocumentTypeId = 4,
                TypeName = "Tax Document",
                IsRequired = false
            }
        );

        // Seed Journey Stages
        modelBuilder.Entity<JourneyStage>().HasData(
            new JourneyStage
            {
                JourneyStageId = 1,
                StageName = "Business Profile",
                InstitutionName = "Business Buzz",
                StageDescription = "Create, review and confirm the entrepreneur's business profile.",
                StageOrder = 1,
                IsRequired = true,
                StageType = "Profile"
            },
            new JourneyStage
            {
                JourneyStageId = 2,
                StageName = "CIPC Registration",
                InstitutionName = "CIPC",
                StageDescription = "Prepare the information and documents required for business registration and guide the entrepreneur to the official CIPC process.",
                StageOrder = 2,
                IsRequired = true,
                StageType = "Institutional"
            },
            new JourneyStage
            {
                JourneyStageId = 3,
                StageName = "SARS Compliance",
                InstitutionName = "SARS",
                StageDescription = "Prepare and track relevant tax and compliance requirements based on the entrepreneur's business circumstances.",
                StageOrder = 3,
                IsRequired = false,
                StageType = "Compliance"
            },
            new JourneyStage
            {
                JourneyStageId = 4,
                StageName = "UIF Compliance",
                InstitutionName = "UIF",
                StageDescription = "Prepare and track UIF-related requirements when applicable, such as when the business employs people.",
                StageOrder = 4,
                IsRequired = false,
                StageType = "Compliance"
            },
            new JourneyStage
            {
                JourneyStageId = 5,
                StageName = "Business Readiness",
                InstitutionName = "Business Buzz",
                StageDescription = "Review outstanding preparation tasks and determine whether the business is ready for the next stage of its journey.",
                StageOrder = 5,
                IsRequired = true,
                StageType = "Readiness"
            },
            new JourneyStage
            {
                JourneyStageId = 6,
                StageName = "Funding Readiness",
                InstitutionName = "Business Buzz",
                StageDescription = "Prepare the business for an optional funding assessment and organise relevant information and documents.",
                StageOrder = 6,
                IsRequired = false,
                StageType = "Funding"
            }
        );
    }

}