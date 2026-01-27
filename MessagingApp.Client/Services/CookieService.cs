using Microsoft.JSInterop;

namespace MessagingApp.Client.Services
{
    public class CookieService
    {
        private readonly IJSRuntime JS;

        public CookieService(IJSRuntime jS)
        {
            JS = jS;
        }

        public async Task StoreToken(string token)
        {
            await JS.InvokeVoidAsync("setCookie", "access_token", token, 1);
        }

        public async Task RemoveToken()
        {
            await JS.InvokeVoidAsync("deleteCookie", "access_token");
        }

        public async Task<string> GetToken()
        {
            return await JS.InvokeAsync<string>("getCookie", "access_token");
        }

    }
}
