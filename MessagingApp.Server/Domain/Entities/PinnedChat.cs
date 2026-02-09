namespace MessagingApp.Server.Domain.Entities
{
    public class PinnedChat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PinnedUserId { get; set; }
        public DateTime PinnedAt { get; set; }
    }

}
