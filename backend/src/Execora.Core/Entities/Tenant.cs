using Execora.Core.Enums;

namespace Execora.Core.Entities;

/// <summary>
/// Represents an organization/customer in the multi-tenant system.
/// </summary>
public class Tenant : SoftDeletableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Organization name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly identifier
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Subscription plan assigned to this tenant
    /// </summary>
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Core;

    /// <summary>
    /// Current status of the subscription
    /// </summary>
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;

    /// <summary>
    /// Plan expiration date
    /// </summary>
    public DateTime? SubscriptionExpiry { get; set; }

    /// <summary>
    /// Maximum number of projects based on plan
    /// </summary>
    public int? MaxProjects { get; set; }

    /// <summary>
    /// Maximum number of users based on plan
    /// </summary>
    public int? MaxUsers { get; set; }

    // Navigation properties
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
