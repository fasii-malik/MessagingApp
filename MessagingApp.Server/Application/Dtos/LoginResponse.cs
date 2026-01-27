namespace MessagingApp.Server.Application.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!; // optional for now
        public DateTime Expiration { get; set; }
    }
}
