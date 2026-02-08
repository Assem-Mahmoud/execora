namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for managing password history
/// </summary>
public interface IPasswordHistoryRepository
{
    /// <summary>
    /// Adds a password to user's history
    /// </summary>
    Task AddPasswordAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent password hashes for a user (last N passwords)
    /// </summary>
    Task<List<string>> GetRecentPasswordHashesAsync(Guid userId, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a password hash exists in user's recent history
    /// </summary>
    Task<bool> IsPasswordInHistoryAsync(Guid userId, string passwordHash, int historyCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes old password history entries beyond the retention limit
    /// </summary>
    Task CleanupOldHistoryAsync(Guid userId, int keepCount, CancellationToken cancellationToken = default);
}
