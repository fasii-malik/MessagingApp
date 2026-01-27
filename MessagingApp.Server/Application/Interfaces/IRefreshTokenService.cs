using MessagingApp.Server.Application.Services;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateAsync(Guid userId);
        Task<RefreshToken?> GetValidAsync(string token);
        Task RevokeAsync(RefreshToken token);
        Task<int> CleanupExpiredTokensAsync();
    }
}
