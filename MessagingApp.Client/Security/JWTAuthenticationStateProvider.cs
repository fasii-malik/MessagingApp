using MessagingApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MessagingApp.Client.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly CookieService _cookieService;
        public bool IsTokenExpired { get; private set; } = false;

        public JWTAuthenticationStateProvider(CookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _cookieService.GetToken();
                
                if (string.IsNullOrEmpty(token))
                    return await MarkAsUnauthorize();


                var readJWT = new JwtSecurityTokenHandler().ReadJwtToken(token);

                // ✅ Check token expiration
                var expClaim = readJWT.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (!string.IsNullOrEmpty(expClaim))
                {
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim));
                    if (expTime < DateTimeOffset.UtcNow)
                    {
                        // Token expired
                        await MarkAsUnauthorize(true); // mark token as expired
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }

                var claims = readJWT.Claims.ToList();

                // Normalize role claim
                var roleClaims = claims.Where(c => c.Type == "role").ToList();
                foreach (var role in roleClaims)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Value));
                }

                var identity = new ClaimsIdentity(claims, "JWT", ClaimTypes.Name, ClaimTypes.Role);

                var principal = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(
    Task.FromResult(new AuthenticationState(principal)));

                return await Task.FromResult(new AuthenticationState(principal));
            }
            catch (Exception ex)
            {
                return await MarkAsUnauthorize();
            }            
        }

        public async Task<AuthenticationState> MarkAsUnauthorize(bool tokenExpired = false)
        {
            try
            {
                IsTokenExpired = tokenExpired;

                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }
            catch (Exception ex)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkAsAuthenticated()
        {
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _cookieService.GetToken();
        }

        public async Task<string?> GetUserIdAsync()
        {
            var token = await _cookieService.GetToken();
            if (string.IsNullOrEmpty(token)) return null;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        }

    }//class
}//namespace
