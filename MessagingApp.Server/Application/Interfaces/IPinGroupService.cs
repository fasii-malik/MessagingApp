namespace MessagingApp.Server.Application.Interfaces
{
    public interface IPinGroupService
    {
        Task PinAsync(Guid userId, Guid groupId);
        Task UnpinAsync(Guid userId, Guid groupId);
    }

}
