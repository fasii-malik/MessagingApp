using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{
    public class AgentChatRequest
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("chat_history")]
        public List<AgentChatHistoryItem> ChatHistory { get; set; } = new();
    }
}
