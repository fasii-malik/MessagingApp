using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IPinnedChatRepository
    {
        Task<PinnedChat?> GetAsync(Guid userId, Guid pinnedUserId);
        Task AddAsync(PinnedChat pin);
        Task RemoveAsync(PinnedChat pin);
        Task<List<Guid>> GetPinnedUserIdsAsync(Guid userId);
    }
}
