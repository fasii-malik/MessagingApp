namespace MessagingApp.Server.Domain.Entities
{
    public class GroupMember
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;

        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

}
