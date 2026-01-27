using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task SaveChangesAsync();

        Task<int> RemoveExpiredTokensAsync();
    }
}
