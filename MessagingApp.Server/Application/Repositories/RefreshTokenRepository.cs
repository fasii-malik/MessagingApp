using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
    .Include(rt => rt.User)
    .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        // Clean up expired and revoked tokens
        public async Task<int> RemoveExpiredTokensAsync()
        {
            var expiredTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync();

            _dbContext.RefreshTokens.RemoveRange(expiredTokens);
            return expiredTokens.Count;
        }
    }
}
