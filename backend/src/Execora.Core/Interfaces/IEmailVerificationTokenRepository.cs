using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository interface for managing email verification tokens
/// </summary>
public interface IEmailVerificationTokenRepository
{
    /// <summary>
    /// Creates a new email verification token
    /// </summary>
    /// <param name="token">The email verification token to create</param>
    /// <returns>The created token</returns>
    Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token);

    /// <summary>
    /// Gets a verification token by its value (token string)
    /// </summary>
    /// <param name="token">The token value to search for</param>
    /// <returns>The verification token if found, null otherwise</returns>
    Task<EmailVerificationToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Gets all verification tokens for a specific email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>List of verification tokens for the email</returns>
    Task<List<EmailVerificationToken>> GetByEmailAsync(string email);

    /// <summary>
    /// Gets all verification tokens for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of verification tokens for the user</returns>
    Task<List<EmailVerificationToken>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Updates an existing verification token
    /// </summary>
    /// <param name="token">The token to update</param>
    /// <returns>The updated token</returns>
    Task<EmailVerificationToken> UpdateAsync(EmailVerificationToken token);

    /// <summary>
    /// Deletes a verification token
    /// </summary>
    /// <param name="token">The token to delete</param>
    Task DeleteAsync(EmailVerificationToken token);

    /// <summary>
    /// Deletes all expired verification tokens
    /// </summary>
    /// <returns>Number of tokens deleted</returns>
    Task<int> DeleteExpiredTokensAsync();

    /// <summary>
    /// Deletes all verification tokens for a specific email address
    /// </summary>
    /// <param name="email">The email address</param>
    Task DeleteByEmailAsync(string email);

    /// <summary>
    /// Deletes all verification tokens for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    Task DeleteByUserIdAsync(Guid userId);
}