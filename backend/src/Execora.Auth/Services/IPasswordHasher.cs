namespace Execora.Auth.Services;

/// <summary>
/// Service for hashing and verifying passwords
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Hashed password</param>
    /// <returns>True if password matches hash</returns>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Validates a password against strength requirements
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <returns>True if password meets requirements</returns>
    bool ValidatePasswordStrength(string password);
}
