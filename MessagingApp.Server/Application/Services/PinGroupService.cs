using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Services
{
    public class PinGroupService : IPinGroupService
    {
        private readonly IPinnedGroupRepository _repo;

        public PinGroupService(IPinnedGroupRepository repo)
        {
            _repo = repo;
        }

        public async Task PinAsync(Guid userId, Guid groupId)
        {
            var existing = await _repo.GetAsync(userId, groupId);
            if (existing != null) return;

            await _repo.AddAsync(new PinnedGroup
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                PinnedAt = DateTime.UtcNow
            });
        }

        public async Task UnpinAsync(Guid userId, Guid groupId)
        {
            var pin = await _repo.GetAsync(userId, groupId);
            if (pin == null) return;

            await _repo.RemoveAsync(pin);
        }
    }

}
