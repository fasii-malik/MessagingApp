using Newtonsoft.Json;

namespace MessagingApp.Client.Models.Auth
{
    public class LoginResponse
    {
        [JsonProperty("token")]
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
