using MessagingApp.Client.Models.Agent;
using Microsoft.AspNetCore.Components.Forms;
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

        public async Task<UploadResponse?> UploadDocumentAsync(IBrowserFile file)
        {
            if (file == null)
                return null;

            try
            {
                using var content = new MultipartFormDataContent();

                // Convert IBrowserFile to StreamContent
                var stream = file.OpenReadStream(); 
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                // Add to form data (key must match FastAPI endpoint)
                content.Add(streamContent, "file", file.Name);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "http://localhost:8001/documents/upload")
                {
                    Content = content
                };

                var response = await _http.SendAsync(httpRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Upload error: " + error);
                    return null;
                }

                // Deserialize response
                var uploadResponse = await response.Content.ReadFromJsonAsync<UploadResponse>();
                Console.WriteLine("Document Embedding Response: " + JsonSerializer.Serialize(uploadResponse, new JsonSerializerOptions { WriteIndented = true }));
                
                return uploadResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception uploading file: " + ex.Message);
                return null;
            }
        }

        public async Task<List<DocumentResponse>> GetUploadedDocumentsAsync()
        {
            try
            {
                // Call FastAPI GET /documents
                var documents = await _http.GetFromJsonAsync<List<DocumentResponse>>("http://localhost:8001/documents");

                // Print the entire JSON list nicely formatted
                var json = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine("Documents Found:\n" + json);

                return documents ?? new List<DocumentResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get uploaded documents: " + ex.Message);
                return new List<DocumentResponse>();
            }
        }


        public async Task<bool> DeleteDocumentAsync(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
                return false;

            try
            {
                // Call FastAPI DELETE endpoint
                var response = await _http.DeleteAsync($"http://localhost:8001/documents/{documentId}");

                if (!response.IsSuccessStatusCode)
                {
                    // Log error
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to delete document: {error}");
                    return false;
                }

                // Optionally deserialize response to get details
                var result = await response.Content.ReadFromJsonAsync<DeleteDocumentResponse>();

                Console.WriteLine("\nDocument Embedding Deleted: "+ result.Message);

                if (result != null && result.Success)
                {
                    Console.WriteLine($"Deleted document: {result.DocumentName}, deleted chunks: {result.DeletedChunks}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting document: {ex.Message}");
                return false;
            }
        }

        public async Task<AgentChatResponse?> SendRagMessageAsync(
    string question,
    string? documentId,
    bool useRag)
        {
            var requestBody = new RagChatRequest
            {
                Question = question,
                DocumentId = documentId,
                UseRag = useRag
            };

            Console.WriteLine("RAG Request: " +
                JsonSerializer.Serialize(requestBody));

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:8001/chat/stream")
            {
                Content = JsonContent.Create(requestBody)
            };

            var response = await _http.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("RAG API error: " + error);
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

                Console.WriteLine("Stream Line: " + line);

                try
                {
                    var chunk = JsonSerializer.Deserialize<AgentStreamChunk>(line, options);

                    if (chunk?.Type == "sources")
                    {
                        // Optional: handle sources
                        Console.WriteLine("Sources received");
                    }

                    if (chunk?.Type == "chunk" && !string.IsNullOrEmpty(chunk.Content))
                    {
                        fullMessage.Append(chunk.Content);
                    }

                    if (chunk?.Type == "done")
                    {
                        break;
                    }

                    if (chunk?.Type == "error")
                    {
                        Console.WriteLine("Stream error: " + chunk.Content);
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
