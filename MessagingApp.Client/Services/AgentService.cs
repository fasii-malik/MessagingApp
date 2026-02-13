using MessagingApp.Client.Models.Agent;
using System.Text;
using System.Text.Json;

namespace MessagingApp.Client.Services
{
    public class AgentService
    {
        private readonly HttpClient _http;      

        public AgentService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("Api");            
        }

        public async Task<List<AgentDto>> GetAgentsAsync()
        {
            var response = await _http.GetFromJsonAsync<List<AgentDto>>("api/User/agents");
            return response ?? new List<AgentDto>();
        }

        public async Task<AgentChatResponse?> SendMessageToAgentAsync(string agentMention, 
            List<AgentChatHistoryItem> chatHistory)
        {
            var requestBody = new AgentChatRequest
            {
                Message = agentMention,
                ChatHistory = chatHistory
            };

            Console.WriteLine("Request Body: " +
    JsonSerializer.Serialize(requestBody));

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:8001/chat/agent-mention")
            {
                Content = JsonContent.Create(requestBody)
            };

            var response = await _http.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead);

            Console.WriteLine("Agent Response: "+response);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Agent API error: {error}");
                return null;
            }

            var fullMessage = new StringBuilder();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            while (true)
            {
                var line = await reader.ReadLineAsync();

                if (line == null)
                    break;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                Console.WriteLine("Raw Line: " + line);

                try
                {
                    var chunk = JsonSerializer.Deserialize<AgentStreamChunk>(line, options);

                    if (chunk?.Type == "chunk" && !string.IsNullOrEmpty(chunk.Content))
                    {
                        fullMessage.Append(chunk.Content);
                    }

                    if (chunk?.Type == "done")
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Deserialize error: " + ex.Message);
                }
            }


            return new AgentChatResponse
            {
                Content = fullMessage.ToString()
            };
        }

    }
}
