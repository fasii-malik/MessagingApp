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

        // ---------- Group Messages ----------
        public async Task ToggleGroupStarAsync(Guid userId, Guid groupMessageId)
        {
            if (await _repo.GroupMessageExistsAsync(userId, groupMessageId))
            {
                await _repo.RemoveGroupMessageStarredAsync(userId, groupMessageId);
            }
            else
            {
                await _repo.AddGroupMessageStarredAsync(new GroupMessageStarred
                {
                    UserId = userId,
                    GroupMessageId = groupMessageId,
                    StarredAt = DateTime.UtcNow
                });
            }
        }

        public async Task<List<StarredMessageDto>> GetStarredGroupMessagesAsync(Guid userId)
        {
            return await _repo.GetStarredGroupMessagesAsync(userId);
        }

        // ---------- Optional: Get All Starred Messages ----------
        public async Task<List<StarredMessageDto>> GetAllStarredMessagesAsync(Guid userId)
        {
            var privateMessages = await _repo.GetStarredMessagesAsync(userId);
            var groupMessages = await _repo.GetStarredGroupMessagesAsync(userId);

            // Combine and order by StarredAt descending
            return privateMessages
                .Concat(groupMessages)
                .OrderByDescending(m => m.StarredAtUtc)
                .ToList();
        }
    }

}
