namespace MessagingApp.Server.Application.Dtos
{
    public class ChatGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedById { get; set; }
        public int MembersCount { get; set; }
        public List<ChatGroupMemberDto> Members { get; set; } = [];

        // last message info
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public Guid? LastMessageSenderId { get; set; }
    }
}
