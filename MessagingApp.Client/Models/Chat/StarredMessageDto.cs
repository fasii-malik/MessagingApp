namespace MessagingApp.Client.Models.Chat
{
    public class StarredMessageDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public DateTime StarredAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
    }

}
