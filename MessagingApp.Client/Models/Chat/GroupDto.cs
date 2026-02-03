namespace MessagingApp.Client.Models.Chat
{
        public class GroupDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;

            public int MembersCount { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageTime { get; set; }

        public DateTime CreatedAt { get; set; }
        }    
}
