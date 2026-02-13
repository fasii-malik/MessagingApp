namespace MessagingApp.Server.Domain.Entities
{
    public class GroupMessage
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;

        public User Sender { get; set; } = null!; 
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}
