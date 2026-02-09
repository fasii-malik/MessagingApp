namespace MessagingApp.Server.Application.Dtos
{
    public class StarredMessageDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public DateTime StarredAtUtc { get; set; }
    }

}
