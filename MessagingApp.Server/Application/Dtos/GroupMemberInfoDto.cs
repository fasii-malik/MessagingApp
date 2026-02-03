namespace MessagingApp.Server.Application.Dtos
{
        public class GroupMemberInfoDto
        {
            public Guid UserId { get; set; }
            public string Username { get; set; } = string.Empty; // new
            public bool IsAdmin { get; set; }
            public DateTime JoinedAt { get; set; }
            public bool IsOnline { get; set; } = false; // new
        }

}
