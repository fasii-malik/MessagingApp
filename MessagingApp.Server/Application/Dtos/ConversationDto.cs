namespace MessagingApp.Server.Application.Dtos
{
    public class ConversationDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public bool IsOnline { get; set; }

        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsPinned { get; set; }
        public int UnreadCount { get; set; }
    }

}
