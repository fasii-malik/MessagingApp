using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{
    public class AgentChatHistoryItem
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    }
}
