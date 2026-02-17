using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{    
    public class RagChatRequest
    {
        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;

        [JsonPropertyName("document_id")]
        public string? DocumentId { get; set; }

        [JsonPropertyName("use_rag")]
        public bool UseRag { get; set; }
    }

}
