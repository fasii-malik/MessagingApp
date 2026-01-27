using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Repositories;
using MessagingApp.Server.Domain.Entities;
using System.Security.Cryptography;

namespace MessagingApp.Server.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _repository;

        public RefreshTokenService(IConfiguration configuration, IRefreshTokenRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public async Task<int> CleanupExpiredTokensAsync()
        {
            return await _repository.RemoveExpiredTokensAsync();
        }

        public async Task<RefreshToken> CreateAsync(Guid userId)
        {
            var refreshToken = new RefreshToken
            {
               UserId = userId,
               Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"])
            )                
            };

            await _repository.AddAsync(refreshToken);
            await _repository.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken?> GetValidAsync(string token)
        {
            var refreshToken = await _repository.GetByTokenAsync(token);

            if (refreshToken is null)
                return null;

            if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
                return null;

            return refreshToken;
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            await _repository.SaveChangesAsync();
        }


    }
}
