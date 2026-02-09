using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IStarredMessageRepository
    {
        Task AddAsync(StarredMessage starredMessage);
        Task RemoveAsync(Guid userId, Guid messageId);
        Task<bool> ExistsAsync(Guid userId, Guid messageId);
        Task<List<StarredMessageDto>> GetStarredMessagesAsync(Guid userId);

    }

}
