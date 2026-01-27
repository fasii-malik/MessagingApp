namespace MessagingApp.Server.Application.Dtos
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
