using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<List<Message>> GetChatAsync(string userAId, string userBId);
        Task<Message?> GetLastMessageAsync(string userAId, string userBId);
        Task<List<User>> GetAllUsersExceptAsync(string currentUserId);
        Task<List<Message>> GetAllMessagesForUserAsync(string currentUserId);
        Task DeleteManyAsync(IEnumerable<Guid> messageIds);
        Task DeleteConversationAsync(Guid userId, Guid otherUserId);

    }
}
