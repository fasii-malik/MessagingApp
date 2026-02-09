using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace MessagingApp.Server.Application.Repositories
{
    public class PinnedGroupRepository : IPinnedGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PinnedGroupRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PinnedGroup?> GetAsync(Guid userId, Guid groupId)
        {
            return await _dbContext.PinnedGroups
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.GroupId == groupId);
        }

        public async Task AddAsync(PinnedGroup pin)
        {
            _dbContext.PinnedGroups.Add(pin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(PinnedGroup pin)
        {
            _dbContext.PinnedGroups.Remove(pin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetPinnedGroupIdsAsync(Guid userId)
        {
            return await _dbContext.PinnedGroups
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PinnedAt)
                .Select(x => x.GroupId)
                .ToListAsync();
        }
    }

}
