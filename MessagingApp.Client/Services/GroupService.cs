using MessagingApp.Client.Models.Chat;
using System.Net.Http.Headers;

namespace MessagingApp.Client.Services
{
    public class GroupService
    {
        private readonly HttpClient _http;
        private readonly ILogger<MessageService> _logger;
        private readonly CookieService _cookieService;

        public GroupService(IHttpClientFactory httpFactory, ILogger<MessageService> logger, CookieService cookieService)
        {
            _http = httpFactory.CreateClient("Api");
            _logger = logger;
            _cookieService = cookieService;
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            var token = await _cookieService.GetToken();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/GroupChat/get-groups");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            return await response.Content.ReadFromJsonAsync<List<GroupDto>>() ?? new();
        }

        public async Task<GroupDto?> CreateGroupAsync(CreateGroupDto dto)
        {
            var token = await _cookieService.GetToken();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/GroupChat/create-groups");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(dto);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GroupDto>();
        }

        public async Task<bool> LeaveGroupAsync(Guid? groupId)
        {
            try
            {
                var token = await _cookieService.GetToken();
                if (string.IsNullOrEmpty(token))
                    return false;

                var request = new HttpRequestMessage(HttpMethod.Post, $"/api/GroupChat/{groupId}/leave");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving group {GroupId}", groupId);
                return false;
            }
        }
        public async Task<GroupInfoDto?> GetGroupInfoAsync(Guid groupId)
        {
            try
            {
                var token = await _cookieService.GetToken();
                if (string.IsNullOrEmpty(token))
                    return null;

                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"/api/GroupChat/{groupId}/info"
                );

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<GroupInfoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group info {GroupId}", groupId);
                return null;
            }
        }

        public async Task<bool> AddMemberAsync(Guid groupId, Guid userId)
        {
            var token = await _cookieService.GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/GroupChat/{groupId}/add-member"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(userId);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

    }
}
