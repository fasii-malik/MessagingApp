using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content);
        Task<List<Message>> GetChatAsync(string userAId, string userBId);
        Task<List<UserDto>> GetAllUsersAsync(string currentUserId);
        Task<List<ConversationDto>> GetConversationsAsync(string currentUserId);
        Task DeleteMessagesAsync(IEnumerable<Guid> messageIds, Guid currentUserId);
        Task DeleteConversationAsync(Guid currentUserId, Guid otherUserId);
    }
}
