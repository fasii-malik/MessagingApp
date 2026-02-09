using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IPinnedGroupRepository
    {
        Task<PinnedGroup?> GetAsync(Guid userId, Guid groupId);
        Task AddAsync(PinnedGroup pin);
        Task RemoveAsync(PinnedGroup pin);
        Task<List<Guid>> GetPinnedGroupIdsAsync(Guid userId);
    }

}
