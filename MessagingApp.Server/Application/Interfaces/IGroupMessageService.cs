using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IGroupMessageService
    {
        Task<GroupMessageDto> SendGroupMessageAsync(Guid groupId, Guid senderId, string content);

        Task<List<GroupMessageDto>> GetGroupMessagesAsync(Guid groupId);
        Task<bool> DeleteGroupMessagesAsync(List<Guid> messageIds);        

    }
}
