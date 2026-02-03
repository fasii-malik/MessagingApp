using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IGroupRepository
    {
        Task<ChatGroup> CreateGroupAsync(string name, Guid creatorId, List<Guid> memberIds);
        Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId);
        Task<bool> RemoveUserFromGroupAsync(Guid groupId, Guid userId);

        Task<int> GetMemberCountAsync(Guid groupId);

        Task<bool> DeleteGroupAsync(Guid groupId);

        Task<ChatGroup?> GetGroupInfoAsync(Guid groupId);

        Task<bool> AddMemberAsync(Guid groupId, Guid userId);
        Task<bool> IsUserAdminAsync(Guid groupId, Guid userId);

    }

}
