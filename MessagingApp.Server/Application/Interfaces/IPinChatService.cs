namespace MessagingApp.Server.Application.Interfaces
{
    public interface IPinChatService
    {
        Task PinAsync(Guid userId, Guid pinnedUserId);
        Task UnpinAsync(Guid userId, Guid pinnedUserId);
    }

}
