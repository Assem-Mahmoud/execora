namespace Execora.Core.Entities;

/// <summary>
/// Represents a company/unit within a tenant (for large multi-org tenants).
/// </summary>
public class Organization : SoftDeletableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Reference to the tenant
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Organization/unit name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Self-reference for hierarchy
    /// </summary>
    public Guid? ParentOrganizationId { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Logo URL
    /// </summary>
    public string? LogoUrl { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Organization? ParentOrganization { get; set; }
    public ICollection<Organization> ChildOrganizations { get; set; } = new List<Organization>();
}
