using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace MessagingApp.Server.Application.Repositories
{
    public class PinnedChatRepository : IPinnedChatRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PinnedChatRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PinnedChat?> GetAsync(Guid userId, Guid pinnedUserId)
        {
            return await _dbContext.PinnedChats
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.PinnedUserId == pinnedUserId);
        }

        public async Task AddAsync(PinnedChat pin)
        {
            _dbContext.PinnedChats.Add(pin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(PinnedChat pin)
        {
            _dbContext.PinnedChats.Remove(pin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetPinnedUserIdsAsync(Guid userId)
        {
            return await _dbContext.PinnedChats
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PinnedAt)
                .Select(x => x.PinnedUserId)
                .ToListAsync();
        }
    }

}
