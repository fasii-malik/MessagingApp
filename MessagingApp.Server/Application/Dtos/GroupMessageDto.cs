using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Dtos
{
    public class GroupMessageDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = ""; // optional if you want name
        public string Content { get; set; } = "";
        public DateTime SentAt { get; set; }
    }
}
