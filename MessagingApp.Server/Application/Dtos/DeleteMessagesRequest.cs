namespace MessagingApp.Server.Application.Dtos
{
    public class DeleteMessagesRequest
    {
        public List<Guid> MessageIds { get; set; } = new();
        
    }
}
