using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IGroupService
    {
        Task<ChatGroupDto> CreateGroupAsync( string name, Guid creatorId, List<Guid> memberIds);

        Task<List<ChatGroupDto>> GetUserGroupsAsync(Guid userId);

        Task<bool> LeaveGroupAsync(Guid groupId, Guid userId);

        Task<GroupInfoDto?> GetGroupInfoAsync(Guid groupId, Guid currentUserId);

        Task<bool> AddMemberAsync(Guid groupId, Guid adminId, Guid userId);

    }
}
