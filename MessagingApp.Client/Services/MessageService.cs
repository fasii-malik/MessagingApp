using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MessagingApp.Client.Models.Chat;
using MessagingApp.Client.Models.User;

namespace MessagingApp.Client.Services
{
    public class MessageService
    {
        private readonly HttpClient _http;
        private readonly ILogger<MessageService> _logger;
        private readonly CookieService _cookieService;

        public MessageService(IHttpClientFactory httpFactory, ILogger<MessageService> logger, CookieService cookieService)
        {
            _http = httpFactory.CreateClient("Api");
            _logger = logger;
            _cookieService = cookieService;
        }

        public async Task<List<UserDto>?> GetOnlineUsersAsync()
        {
            var token = await _cookieService.GetToken();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/Chat/online-users"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "GetOnlineUsers failed with status {Status}",
                    response.StatusCode
                );
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<UserDto>>();
        }

        public async Task<List<MessageDto>?> GetChatAsync(Guid otherUserId)
        {
            var token = await _cookieService.GetToken();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Chat/{otherUserId}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "GetChat failed with status {Status}",
                    response.StatusCode
                );
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<MessageDto>>();
        }

        public async Task<List<ConversationDto>?> GetConversationAsync()
        {
            var token = await _cookieService.GetToken();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/chat/conversations"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "GetChat failed with status {Status}",
                    response.StatusCode
                );
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<ConversationDto>>();
        }


    }//class
}//namespace
