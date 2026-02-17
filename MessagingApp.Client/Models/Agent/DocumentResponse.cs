using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{    
    public class DocumentResponse
    {
        [JsonPropertyName("document_id")]
        public string DocumentId { get; set; } = string.Empty;

        [JsonPropertyName("document_name")]
        public string DocumentName { get; set; } = string.Empty;

        [JsonPropertyName("total_chunks")]
        public int TotalChunks { get; set; }

        [JsonPropertyName("upload_timestamp")]
        public string UploadTimestamp { get; set; } = string.Empty;
    }

}
