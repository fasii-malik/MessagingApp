using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Services
{
    public class PinChatService : IPinChatService
    {
        private readonly IPinnedChatRepository _repo;

        public PinChatService(IPinnedChatRepository repo)
        {
            _repo = repo;
        }

        public async Task PinAsync(Guid userId, Guid pinnedUserId)
        {
            if (userId == pinnedUserId) return;

            var existing = await _repo.GetAsync(userId, pinnedUserId);
            if (existing != null) return;

            await _repo.AddAsync(new PinnedChat
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PinnedUserId = pinnedUserId,
                PinnedAt = DateTime.UtcNow
            });
        }

        public async Task UnpinAsync(Guid userId, Guid pinnedUserId)
        {
            var pin = await _repo.GetAsync(userId, pinnedUserId);
            if (pin == null) return;

            await _repo.RemoveAsync(pin);
        }
    }

}
