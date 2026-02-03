using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GroupRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatGroup> CreateGroupAsync(string name, Guid creatorId, List<Guid> memberIds)
        {
            var group = new ChatGroup
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedById = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            // Creator as admin
            group.Members.Add(new GroupMember
            {
                UserId = creatorId,
                IsAdmin = true,
                JoinedAt = DateTime.UtcNow
            });

            // Add members
            foreach (var userId in memberIds.Distinct())
            {
                if (userId == creatorId) continue;

                group.Members.Add(new GroupMember
                {
                    UserId = userId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                });
            }

            _dbContext.ChatGroups.Add(group);
            await _dbContext.SaveChangesAsync();

            return group;
        }

        public async Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId)
        {
            return await _dbContext.ChatGroups
                .Include(g => g.Members)
                .Where(g => g.Members.Any(m => m.UserId == userId))
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> RemoveUserFromGroupAsync(Guid groupId, Guid userId)
        {
            var membership = await _dbContext.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (membership == null)
                return false;

            _dbContext.GroupMembers.Remove(membership);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetMemberCountAsync(Guid groupId)
        {
            // Efficiently counts members without loading all their data
            return await _dbContext.GroupMembers
                .CountAsync(gm => gm.GroupId == groupId);
        }

        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            var group = await _dbContext.ChatGroups.FindAsync(groupId);
            if (group == null) return false;

            // EF Core handles cascading deletion of members/messages 
            // if configured in your DbContext
            _dbContext.ChatGroups.Remove(group);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<ChatGroup?> GetGroupInfoAsync(Guid groupId)
        {
            return await _dbContext.ChatGroups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<bool> AddMemberAsync(Guid groupId, Guid userId)
        {
            var exists = await _dbContext.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (exists)
                return false;

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            };

            _dbContext.GroupMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsUserAdminAsync(Guid groupId, Guid userId)
        {
            return await _dbContext.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsAdmin);
        }

    }
}
