namespace MessagingApp.Server.Domain.Entities
{
    public class PinnedGroup
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime PinnedAt { get; set; }
    }

}
