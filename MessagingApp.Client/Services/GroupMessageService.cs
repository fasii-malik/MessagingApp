using MessagingApp.Client.Models.Chat;
using System.Net.Http.Headers;

namespace MessagingApp.Client.Services
{
    public class GroupMessageService
    {
        private readonly HttpClient _http;
        private readonly ILogger<MessageService> _logger;
        private readonly CookieService _cookieService;

        public GroupMessageService(IHttpClientFactory httpFactory, ILogger<MessageService> logger, CookieService cookieService)
        {
            _http = httpFactory.CreateClient("Api");
            _logger = logger;
            _cookieService = cookieService;
        }

        public async Task<List<GroupMessageDto>?> GetGroupMessagesAsync(Guid groupId)
        {
            var token = await _cookieService.GetToken();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/GroupChat/{groupId}/messages" // endpoint for group messages
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "GetGroupMessages failed for group {GroupId} with status {Status}",
                    groupId,
                    response.StatusCode
                );
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<GroupMessageDto>>();
        }

        public async Task<bool> DeleteGroupMessagesAsync(IEnumerable<Guid> ids)
        {
            var response = await _http.PostAsJsonAsync(
                "api/GroupChat/delete",
                ids
            );

            return response.IsSuccessStatusCode;
        }

    }
}
