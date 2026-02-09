using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace MessagingApp.Server.Application.Repositories
{
    public class StarredMessageRepository : IStarredMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StarredMessageRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddAsync(StarredMessage starredMessage)
        {
            _dbContext.StarredMessages.Add(starredMessage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid userId, Guid messageId)
        {
            var starred = await _dbContext.StarredMessages
                .FirstOrDefaultAsync(x => x.UserId == userId && x.MessageId == messageId);

            if (starred != null)
            {
                _dbContext.StarredMessages.Remove(starred);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<bool> ExistsAsync(Guid userId, Guid messageId)
        {
            return _dbContext.StarredMessages
                .AnyAsync(x => x.UserId == userId && x.MessageId == messageId);
        }

        public async Task<List<StarredMessageDto>> GetStarredMessagesAsync(Guid userId)
        {
            return await _dbContext.StarredMessages
                .Where(x => x.UserId == userId)
                .Include(x => x.Message)
                .OrderByDescending(x => x.StarredAt)
                .Select(x => new StarredMessageDto
                {
                    MessageId = x.MessageId,
                    Content = x.Message.Content,
                    SenderId = x.Message.SenderId,
                    StarredAtUtc = x.StarredAt
                })
                .ToListAsync();
        }

    }

}
