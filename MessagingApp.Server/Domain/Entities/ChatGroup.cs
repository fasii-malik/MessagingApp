namespace MessagingApp.Server.Domain.Entities
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    }

}
