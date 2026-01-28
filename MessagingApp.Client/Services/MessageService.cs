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

        public async Task<bool> DeleteMessagesAsync(IEnumerable<Guid> messageIds)
        {
            var token = await _cookieService.GetToken();

            if (string.IsNullOrWhiteSpace(token))
                return false;

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                "/api/Chat/delete-messages"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // DELETE with body (allowed, but must be explicit)
            request.Content = JsonContent.Create(new
            {
                messageIds = messageIds
            });

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "DeleteMessages failed with status {Status}",
                    response.StatusCode
                );
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteConversationAsync(Guid otherUserId)
        {
            try
            {
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                    return false;

                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"/api/Chat/delete-conversation/{otherUserId}"
                );

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("DeleteConversation failed with status {Status}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in DeleteConversationAsync");
                return false;
            }
        }

    }//class
}//namespace
