namespace Execora.Core.Entities;

/// <summary>
/// Base entity with common audit properties
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Base entity with soft delete support
/// </summary>
public abstract class SoftDeletableEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
}
