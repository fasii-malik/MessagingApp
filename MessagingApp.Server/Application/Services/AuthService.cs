using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Repositories;
using MessagingApp.Server.Application.Security;
using MessagingApp.Server.Domain.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MessagingApp.Server.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration, IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLower());

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            // Get user permissions
            var userPermissions = user.Permissions.Select(p => p.Permission).ToList();

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.FullName,user.Role.ToString(), userPermissions,out var expiration);

            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);

            return new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<RefreshTokenResponse?> RefreshAsync(string refreshToken)
        {
            var token = await _refreshTokenService.GetValidAsync(refreshToken);
            if (token is null)
                throw new ApplicationException("Failed to generate access token");

            await _refreshTokenService.RevokeAsync(token);

            var user = token.User;

            // Get user permissions
            var userPermissions = user.Permissions.Select(p => p.Permission).ToList();

            var newAccessToken = _jwtTokenGenerator.GenerateToken(
                user.Id, user.Email, user.FullName,user.Role.ToString(), userPermissions,out var expiration);

            var newRefreshToken = await _refreshTokenService.CreateAsync(user.Id);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                Expiration = expiration  //Access Token Expiration
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ApplicationException("Failed to generate access token");

            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token is null)
                throw new ApplicationException("Failed to generate access token");

            if (token.IsRevoked)
                throw new ApplicationException("Failed to generate access token");

            token.IsRevoked = true;
            //token.RevokedAt = DateTime.UtcNow;
            //token.RevokedReason = "User logout";

            await _refreshTokenRepository.SaveChangesAsync();
        }
    }// class
}// namespace
