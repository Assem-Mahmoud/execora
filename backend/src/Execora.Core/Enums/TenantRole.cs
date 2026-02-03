namespace Execora.Core.Enums;

/// <summary>
/// Roles available within a tenant
/// </summary>
public enum TenantRole
{
    /// <summary>
    /// EXECORA platform admin (global scope)
    /// </summary>
    SystemAdmin = 1,

    /// <summary>
    /// Customer organization admin (tenant scope)
    /// </summary>
    TenantAdmin = 2,

    /// <summary>
    /// Project-level configuration access
    /// </summary>
    ProjectAdmin = 3,

    /// <summary>
    /// Project execution and management
    /// </summary>
    ProjectManager = 4,

    /// <summary>
    /// Quality control and inspection
    /// </summary>
    QAQC = 5,

    /// <summary>
    /// Site operations and reporting
    /// </summary>
    SiteEngineer = 6
}
