using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{
    public class UploadResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("document_id")]
        public string DocumentId { get; set; }

        [JsonPropertyName("document_name")]
        public string DocumentName { get; set; }

        [JsonPropertyName("total_chunks")]
        public int TotalChunks { get; set; }
    }

}
