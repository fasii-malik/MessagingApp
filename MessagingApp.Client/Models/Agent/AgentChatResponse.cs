namespace MessagingApp.Client.Models.Agent
{
    public class AgentChatResponse
    {
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
