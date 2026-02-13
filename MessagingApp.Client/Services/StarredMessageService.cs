using MessagingApp.Client.Models.Chat;
using System.Net.Http.Headers;

namespace MessagingApp.Client.Services
{
    public class StarredMessageService
    {
        private readonly HttpClient _http;
        private readonly ILogger<StarredMessageService> _logger;
        private readonly CookieService _cookieService;

        public StarredMessageService(IHttpClientFactory httpFactory, ILogger<StarredMessageService> logger, CookieService cookieService)
        {
            _http = httpFactory.CreateClient("Api");
            _logger = logger;
            _cookieService = cookieService;
        }

        // Toggle star/unstar a message
        public async Task<bool> ToggleStarAsync(Guid messageId)
        {
            var token = await _cookieService.GetToken();
            if (string.IsNullOrWhiteSpace(token)) return false;

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/StarredMessages/{messageId}"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Toggle star/unstar a group message
        public async Task<bool> ToggleGroupStarAsync(Guid groupMessageId)
        {
            var token = await _cookieService.GetToken();
            if (string.IsNullOrWhiteSpace(token)) return false;

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/StarredMessages/group/{groupMessageId}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Get all starred messages (private + group)
        public async Task<List<StarredMessageDto>> GetAllStarredMessagesAsync()
        {
            var token = await _cookieService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                return new List<StarredMessageDto>();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/StarredMessages/all"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch all starred messages: {StatusCode}",
                    response.StatusCode
                );
                return new List<StarredMessageDto>();
            }

            var messages =
                await response.Content.ReadFromJsonAsync<List<StarredMessageDto>>();

            return messages ?? new List<StarredMessageDto>();
        }

    }
}
