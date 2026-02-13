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

        // ---------- Private Messages ----------
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
                .ThenInclude(m => m.Sender)
                .OrderByDescending(x => x.StarredAt)
                .Select(x => new StarredMessageDto
                {
                    MessageId = x.MessageId,
                    Content = x.Message.Content,
                    SenderId = x.Message.SenderId,
                    StarredAtUtc = x.StarredAt,
                    Fullname = x.Message.Sender.FullName,
                    Type = "Private Message"
                })
                .ToListAsync();
        }
        // ---------- Group Messages ----------
        public async Task AddGroupMessageStarredAsync(GroupMessageStarred starredMessage)
        {
            _dbContext.GroupMessageStarreds.Add(starredMessage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveGroupMessageStarredAsync(Guid userId, Guid groupMessageId)
        {
            var starred = await _dbContext.GroupMessageStarreds
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GroupMessageId == groupMessageId);

            if (starred != null)
            {
                _dbContext.GroupMessageStarreds.Remove(starred);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<bool> GroupMessageExistsAsync(Guid userId, Guid groupMessageId)
        {
            return _dbContext.GroupMessageStarreds
                .AnyAsync(x => x.UserId == userId && x.GroupMessageId == groupMessageId);
        }

        public async Task<List<StarredMessageDto>> GetStarredGroupMessagesAsync(Guid userId)
        {
            return await _dbContext.GroupMessageStarreds
                .Where(x => x.UserId == userId)
                .Include(x => x.GroupMessage)
                .ThenInclude(m => m.Sender)
                .OrderByDescending(x => x.StarredAt)
                .Select(x => new StarredMessageDto
                {
                    MessageId = x.GroupMessageId,
                    Content = x.GroupMessage.Content,
                    SenderId = x.GroupMessage.SenderId,
                    StarredAtUtc = x.StarredAt,
                    Fullname = x.GroupMessage.Sender.FullName,
                    Type = "Group Message"
                })
                .ToListAsync();
        }

    }

}
