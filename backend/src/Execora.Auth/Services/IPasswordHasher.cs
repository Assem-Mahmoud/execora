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

    /// <summary>
    /// Checks if a password hash exists in the user's password history
    /// </summary>
    /// <param name="passwordHash">Password hash to check</param>
    /// <param name="historyPasswords">List of password hashes from history</param>
    /// <returns>True if password is found in history</returns>
    bool IsPasswordInHistory(string passwordHash, IEnumerable<string> historyPasswords);

    /// <summary>
    /// Adds a password hash to the user's password history
    /// </summary>
    /// <param name="passwordHash">Password hash to add to history</param>
    /// <returns>The password hash to store in history</returns>
    string AddToPasswordHistory(string passwordHash);
}
