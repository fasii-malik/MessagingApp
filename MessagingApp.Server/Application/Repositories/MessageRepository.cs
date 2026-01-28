using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public MessageRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Message message)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Message>> GetChatAsync(string userAId, string userBId)
        {
            return await _db.Messages
                .Where(m =>
                    (m.SenderId.ToString() == userAId && m.ReceiverId.ToString() == userBId) ||
                    (m.SenderId.ToString() == userBId && m.ReceiverId.ToString() == userAId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
        public async Task<Message?> GetLastMessageAsync(string userAId, string userBId)
        {
            return await _db.Messages
                .Where(m =>
                    (m.SenderId.ToString() == userAId && m.ReceiverId.ToString() == userBId) ||
                    (m.SenderId.ToString() == userBId && m.ReceiverId.ToString() == userAId))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<List<User>> GetAllUsersExceptAsync(string currentUserId)
        {
            // 1. Parse the string ID into a Guid before the query
            if (!Guid.TryParse(currentUserId, out var currentUserGuid))
            {
                // Return all users if currentUserId is invalid, or handle error
                return await _db.Users.ToListAsync();
            }

            // 2. Perform the query using Guid comparison (High Performance)
            return await _db.Users
                .Where(u => u.Id != currentUserGuid)
                .ToListAsync();
        }
        public async Task<List<Message>> GetAllMessagesForUserAsync(string currentUserId)
        {
            var userGuid = Guid.Parse(currentUserId);

            return await _db.Messages
                .Where(m => m.SenderId == userGuid || m.ReceiverId == userGuid)
                .OrderByDescending(m => m.CreatedAt) // optional: descending for convenience
                .ToListAsync();
        }
        // 🔹 Delete multiple selected messages
        public async Task DeleteManyAsync(IEnumerable<Guid> messageIds)
        {
            var messages = await _db.Messages
                .Where(m => messageIds.Contains(m.Id))
                .ToListAsync();

            if (!messages.Any())
                return;

            _db.Messages.RemoveRange(messages);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteConversationAsync(Guid userId, Guid otherUserId)
        {
            var messages = await _db.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .ToListAsync();

            if (!messages.Any())
                return;

            _db.Messages.RemoveRange(messages);
            await _db.SaveChangesAsync();
        }

    }
}
