using System.Text.Json.Serialization;

namespace MessagingApp.Client.Models.Agent
{

    public class DeleteDocumentResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("document_name")]
        public string DocumentName { get; set; } = string.Empty;

        [JsonPropertyName("deleted_chunks")]
        public int DeletedChunks { get; set; }
    }

}

