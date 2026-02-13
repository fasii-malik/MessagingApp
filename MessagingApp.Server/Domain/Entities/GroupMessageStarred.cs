namespace MessagingApp.Server.Domain.Entities
{
    public class GroupMessageStarred
    {
        public Guid UserId { get; set; }
        public Guid GroupMessageId { get; set; }
        public DateTime StarredAt { get; set; }

        public GroupMessage GroupMessage { get; set; } // navigation property
    }
}
