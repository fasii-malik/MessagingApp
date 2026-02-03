namespace MessagingApp.Server.Application.Dtos
{
    public class SendGroupMessageRequest
    {
        public Guid SenderId { get; set; }
        public string Content { get; set; } = "";
    }
}
