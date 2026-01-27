namespace MessagingApp.Server.Application.Dtos
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string SenderName { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsFromCurrentUser { get; set; }
    }

}
