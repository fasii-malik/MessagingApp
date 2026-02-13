namespace MessagingApp.Client.Models.Chat
{
    public class GroupMessageDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = "";
        public DateTime SentAt { get; set; }

        public bool IsStarred { get; set; } = false; // <-- Add this
    }

}
