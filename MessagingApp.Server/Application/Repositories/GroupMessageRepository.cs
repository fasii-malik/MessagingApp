using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Repositories
{
    public class GroupMessageRepository : IGroupMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GroupMessageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(GroupMessage message)
        {
                _dbContext.GroupMessages.Add(message);
                await _dbContext.SaveChangesAsync();            
        }

        public async Task<List<GroupMessage>> GetGroupMessagesAsync(Guid groupId)
        {
            return await _dbContext.GroupMessages
                .Where(m => m.GroupId == groupId)        // only messages for this group
                .OrderBy(m => m.SentAt)                  // order by sent time
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, GroupMessage>> GetLastMessagesAsync(List<Guid> groupIds)
        {
            return await _dbContext.GroupMessages
                .Where(m => groupIds.Contains(m.GroupId))
                .GroupBy(m => m.GroupId)
                .Select(g => g
                    .OrderByDescending(m => m.SentAt)
                    .First())
                .ToDictionaryAsync(m => m.GroupId);
        }

        public async Task<List<Guid>> DeleteGroupMessagesAsync(List<Guid> messageIds)
        {
            var messages = await _dbContext.GroupMessages
                .Where(m => messageIds.Contains(m.Id))
                .ToListAsync();

            var groupIds = messages
                .Select(m => m.GroupId)
                .Distinct()
                .ToList();

            _dbContext.GroupMessages.RemoveRange(messages);
            await _dbContext.SaveChangesAsync();

            return groupIds;
        }

        public async Task<int> GetMessageCountAsync(Guid groupId)
        {
            return await _dbContext.GroupMessages.CountAsync(m => m.GroupId == groupId);
        }


    }
}
