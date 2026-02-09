using MessagingApp.Server.Application.Dtos;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IStarredMessageService
    {
        Task ToggleStarAsync(Guid userId, Guid messageId);
        Task<List<StarredMessageDto>> GetStarredMessagesAsync(Guid userId);
    }

}
