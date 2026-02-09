using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Services
{
    public class StarredMessageService : IStarredMessageService
    {
        private readonly IStarredMessageRepository _repo;

        public StarredMessageService(IStarredMessageRepository repo)
        {
            _repo = repo;
        }

        public async Task ToggleStarAsync(Guid userId, Guid messageId)
        {
            if (await _repo.ExistsAsync(userId, messageId))
            {
                await _repo.RemoveAsync(userId, messageId);
            }
            else
            {
                await _repo.AddAsync(new StarredMessage
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    MessageId = messageId,
                    StarredAt = DateTime.UtcNow
                });
            }
        }

        public async Task<List<StarredMessageDto>> GetStarredMessagesAsync(Guid userId)
        {
            return await _repo.GetStarredMessagesAsync(userId);
        }

    }

}
