using MessagingApp.Client.Models.Auth;
using MessagingApp.Client.Security;
using Newtonsoft.Json;

namespace MessagingApp.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly CookieService _cookieService;
        private readonly JWTAuthenticationStateProvider _authStateProvider;


        public AuthService(IHttpClientFactory factory, CookieService cookieService, JWTAuthenticationStateProvider authStateProvider)
        {
            _http = factory.CreateClient("Api");
            _cookieService = cookieService;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest model)
        {            
                Console.WriteLine($"Attempting login for: {model.Email}");
                Console.WriteLine($"Base Address: {_http.BaseAddress}");

                var response = await _http.PostAsJsonAsync("/api/Auth/login", model);

                if(response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LoginResponse>(token);
                    await _cookieService.StoreToken(result.AccessToken);

                    await _authStateProvider.MarkAsAuthenticated();
            }


                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                return response.IsSuccessStatusCode;            
        }

        public async Task<bool> RegisterAsync(RegisterRequest model)
        {
                Console.WriteLine($"Attempting login for: {model.Email}");
                Console.WriteLine($"Base Address: {_http.BaseAddress}");

                var response = await _http.PostAsJsonAsync("/api/Auth/register", model);

                Console.WriteLine($"Response Status: {response.StatusCode}");


                return response.IsSuccessStatusCode;            
        }
    }

}
