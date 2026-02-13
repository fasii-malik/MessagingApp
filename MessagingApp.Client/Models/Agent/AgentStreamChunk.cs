using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{
    public class AgentStreamChunk
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

}
