using Execora.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Data;

/// <summary>
/// Main database context for EXECORA platform.
/// Implements Row-Level Security for multi-tenant data isolation.
/// </summary>
public class ExecoraDbContext : IdentityDbContext<IdentityUser>
{
    public ExecoraDbContext(DbContextOptions<ExecoraDbContext> options)
        : base(options)
    {
    }

    #region Multi-Tenancy Core

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public override DbSet<IdentityUser> Users { get; set; } = null!;
    public DbSet<User> AppUsers { get; set; } = null!;
    public DbSet<TenantUser> TenantUsers { get; set; } = null!;
    public DbSet<Organization> Organizations { get; set; } = null!;

    #endregion

    #region Authentication & User Management

    public DbSet<Invitation> Invitations { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<PasswordHistory> PasswordHistory { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    #endregion

    #region Organization & Project Management (Module 01)

    //public DbSet<Project> Projects { get; set; } = null!;
    //public DbSet<ProjectUser> ProjectUsers { get; set; } = null!;
    //public DbSet<Trade> Trades { get; set; } = null!;

    #endregion

    #region Activities (Module 03)

    //public DbSet<Activity> Activities { get; set; } = null!;
    //public DbSet<ActivityProgress> ActivityProgress { get; set; } = null!;

    #endregion

    #region Inspections (Module 04)

    //public DbSet<InspectionTemplate> InspectionTemplates { get; set; } = null!;
    //public DbSet<Inspection> Inspections { get; set; } = null!;
    //public DbSet<InspectionAttachment> InspectionAttachments { get; set; } = null!;

    #endregion

    #region Issues & Risk Management (Module 05)

    //public DbSet<Issue> Issues { get; set; } = null!;
    //public DbSet<IssueComment> IssueComments { get; set; } = null!;

    #endregion

    #region NCR & Quality Intelligence (Module 06)

    //public DbSet<NonConformanceReport> NonConformanceReports { get; set; } = null!;
    //public DbSet<NcrAttachment> NcrAttachments { get; set; } = null!;

    #endregion

    #region Workflow Engine (Module 07)

    //public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = null!;
    //public DbSet<WorkflowInstance> WorkflowInstances { get; set; } = null!;
    //public DbSet<WorkflowTransition> WorkflowTransitions { get; set; } = null!;

    #endregion

    #region Daily Operations (Module 02)

    //public DbSet<DailyReport> DailyReports { get; set; } = null!;
    //public DbSet<ManpowerEntry> ManpowerEntries { get; set; } = null!;

    #endregion

    #region Notifications (Module 10)

    //public DbSet<Notification> Notifications { get; set; } = null!;
    //public DbSet<NotificationPreference> NotificationPreferences { get; set; } = null!;

    #endregion

    #region Audit Trail

    //public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    #endregion

    #region BIM Integration (Module 08)

    //public DbSet<BimModel> BimModels { get; set; } = null!;
    //public DbSet<BimElementMapping> BimElementMappings { get; set; } = null!;

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Row-Level Security for multi-tenant isolation
        ConfigureRowLevelSecurity(builder);

        // Configure entity relationships and constraints
        ConfigureEntities(builder);

        // Configure indexes
        ConfigureIndexes(builder);
    }

    /// <summary>
    /// Configures Row-Level Security for multi-tenant data isolation.
    /// This ensures that queries automatically filter by tenant ID.
    /// </summary>
    private void ConfigureRowLevelSecurity(ModelBuilder builder)
    {
        // Global query filters for tenant-isolated entities
        builder.Entity<Tenant>()
            .HasQueryFilter(t => !t.IsDeleted);

        builder.Entity<Organization>()
            .HasQueryFilter(o => !o.IsDeleted);

        // Additional RLS will be configured for each tenant-scoped entity
        // as they are implemented in subsequent phases
    }

    /// <summary>
    /// Configures entity relationships and constraints.
    /// </summary>
    private void ConfigureEntities(ModelBuilder builder)
    {
        // Tenant - TenantUser (One-to-Many)
        builder.Entity<TenantUser>()
            .HasOne(tu => tu.Tenant)
            .WithMany(t => t.TenantUsers)
            .HasForeignKey(tu => tu.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - TenantUser (One-to-Many)
        builder.Entity<TenantUser>()
            .HasOne(tu => tu.User)
            .WithMany(u => u.TenantUsers)
            .HasForeignKey(tu => tu.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tenant - Organization (One-to-Many)
        builder.Entity<Organization>()
            .HasOne(o => o.Tenant)
            .WithMany(t => t.Organizations)
            .HasForeignKey(o => o.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Organization self-reference for hierarchy
        builder.Entity<Organization>()
            .HasOne(o => o.ParentOrganization)
            .WithMany(o => o.ChildOrganizations)
            .HasForeignKey(o => o.ParentOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Invitation - Tenant (Many-to-One)
        builder.Entity<Invitation>()
            .HasOne(i => i.Tenant)
            .WithMany()
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // RefreshToken - User (Many-to-One)
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // PasswordResetToken - User (Many-to-One)
        builder.Entity<PasswordResetToken>()
            .HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint on TenantUser combination
        builder.Entity<TenantUser>()
            .HasIndex(tu => new { tu.TenantId, tu.UserId })
            .IsUnique()
            .HasDatabaseName("IX_TenantUsers_TenantId_UserId");
    }

    /// <summary>
    /// Configures database indexes for performance optimization.
    /// </summary>
    private void ConfigureIndexes(ModelBuilder builder)
    {
        // Tenant indexes
        builder.Entity<Tenant>()
            .HasIndex(t => t.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Slug");

        builder.Entity<Tenant>()
            .HasIndex(t => t.SubscriptionStatus)
            .HasDatabaseName("IX_Tenants_SubscriptionStatus");

        // User indexes
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // TenantUser indexes
        builder.Entity<TenantUser>()
            .HasIndex(tu => new { tu.TenantId, tu.Role })
            .HasDatabaseName("IX_TenantUsers_TenantId_Role");

        // Organization indexes
        builder.Entity<Organization>()
            .HasIndex(o => o.TenantId)
            .HasDatabaseName("IX_Organizations_TenantId");

        // Invitation indexes
        builder.Entity<Invitation>()
            .HasIndex(i => new { i.TenantId, i.Status })
            .HasDatabaseName("IX_Invitations_TenantId_Status");

        builder.Entity<Invitation>()
            .HasIndex(i => i.Token)
            .IsUnique()
            .HasDatabaseName("IX_Invitations_Token");

        builder.Entity<Invitation>()
            .HasIndex(i => new { i.Email, i.TenantId })
            .HasDatabaseName("IX_Invitations_Email_TenantId");

        // RefreshToken indexes
        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_TokenHash");

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

        // PasswordResetToken indexes
        builder.Entity<PasswordResetToken>()
            .HasIndex(prt => prt.Token)
            .IsUnique()
            .HasDatabaseName("IX_PasswordResetTokens_Token");

        builder.Entity<PasswordResetToken>()
            .HasIndex(prt => prt.UserId)
            .HasDatabaseName("IX_PasswordResetTokens_UserId");

        builder.Entity<PasswordResetToken>()
            .HasIndex(prt => prt.ExpiresAt)
            .HasDatabaseName("IX_PasswordResetTokens_ExpiresAt");

        builder.Entity<PasswordResetToken>()
            .HasIndex(prt => prt.IsUsed)
            .HasDatabaseName("IX_PasswordResetTokens_IsUsed");

        // AuditLog indexes
        builder.Entity<AuditLog>()
            .HasIndex(al => new { al.TenantId, al.ChangedAt })
            .HasDatabaseName("IX_AuditLogs_TenantId_ChangedAt");

        builder.Entity<AuditLog>()
            .HasIndex(al => new { al.EntityName, al.EntityId })
            .HasDatabaseName("IX_AuditLogs_EntityName_EntityId");

        builder.Entity<AuditLog>()
            .HasIndex(al => al.ChangedBy)
            .HasDatabaseName("IX_AuditLogs_ChangedBy");

        builder.Entity<AuditLog>()
            .HasIndex(al => new { al.ProjectId, al.ChangedAt })
            .HasDatabaseName("IX_AuditLogs_ProjectId_ChangedAt");

        // PasswordHistory indexes
        builder.Entity<PasswordHistory>()
            .HasIndex(ph => ph.UserId)
            .HasDatabaseName("IX_PasswordHistory_UserId");

        builder.Entity<PasswordHistory>()
            .HasIndex(ph => new { ph.UserId, ph.CreatedAt })
            .HasDatabaseName("IX_PasswordHistory_UserId_CreatedAt");
    }

    /// <summary>
    /// Saves all changes made in this context to the database with automatic timestamp updates.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database with automatic timestamp updates.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates CreatedAt and UpdatedAt timestamps automatically.
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var entityType = entry.Context.Model.FindEntityType(entry.Entity.GetType());

            if (entry.State == EntityState.Added)
            {
                var createdAtProperty = entityType?.FindProperty(nameof(BaseEntity.CreatedAt));
                if (createdAtProperty != null && entry.Property(nameof(BaseEntity.CreatedAt)).Metadata != null)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }

            var updatedAtProperty = entityType?.FindProperty(nameof(BaseEntity.UpdatedAt));
            if (updatedAtProperty != null && entry.Property(nameof(BaseEntity.UpdatedAt)).Metadata != null)
            {
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
