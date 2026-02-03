using MessagingApp.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IGroupMessageRepository
    {
        Task AddAsync(GroupMessage message);
        Task<List<GroupMessage>> GetGroupMessagesAsync(Guid groupId);

        Task<Dictionary<Guid, GroupMessage>> GetLastMessagesAsync(List<Guid> groupIds);
        Task<List<Guid>> DeleteGroupMessagesAsync(List<Guid> messageIds);

        Task<int> GetMessageCountAsync(Guid groupId);

    }

}
