using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task CreateAsync(PasswordResetToken token);
    Task MarkAsUsedAsync(Guid tokenId);
    Task DeleteExpiredTokensAsync();
    Task DeleteByUserIdAsync(Guid userId);
}