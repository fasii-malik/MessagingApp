using MessagingApp.Server.Application.Dtos;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IStarredMessageService
    {
        Task ToggleStarAsync(Guid userId, Guid messageId);
        Task<List<StarredMessageDto>> GetStarredMessagesAsync(Guid userId);
        Task ToggleGroupStarAsync(Guid userId, Guid groupMessageId);
        Task<List<StarredMessageDto>> GetStarredGroupMessagesAsync(Guid userId);
        Task<List<StarredMessageDto>> GetAllStarredMessagesAsync(Guid userId);
    }

}
