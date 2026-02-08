using System.Text.RegularExpressions;

namespace Execora.Auth.Services;

/// <summary>
/// Password hashing service using BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;
    private const int MinPasswordLength = 12;
    private const int MaxPasswordLength = 128;

    // At least one uppercase, one lowercase, one digit, one special character
    // Includes commonly used special characters: @ $ ! % * ? & # ^ ~ - _ + = .
    private static readonly Regex PasswordRegex = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^~\-_=.])[A-Za-z\d@$!%*?&#^~\-_=.]{12,}$",
        RegexOptions.Compiled
    );

    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a password against strength requirements
    /// </summary>
    public bool ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
        {
            return false;
        }

        return PasswordRegex.IsMatch(password);
    }
}
