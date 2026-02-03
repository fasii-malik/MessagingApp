namespace MessagingApp.Client.Models.Chat
{
    public class GroupInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MembersCount { get; set; }
        public int MessagesCount { get; set; } // new
        public List<GroupMemberInfoDto> Members { get; set; } = new();
    }
}
