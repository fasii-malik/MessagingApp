using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Repositories;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IGroupMessageRepository _groupMessageRepo;
        private readonly IUserRepository _userRepository;
        private readonly IUserConnectionService _connectionService;
        private readonly IPinnedGroupRepository _pinnedGroupRepository;

        public GroupService(IGroupRepository groupRepo, IGroupMessageRepository groupMessageRepo, IUserRepository userRepository, IUserConnectionService connectionService, IPinnedGroupRepository pinnedGroupRepository)
        {
            _groupRepo = groupRepo;
            _groupMessageRepo = groupMessageRepo;
            _userRepository = userRepository;
            _connectionService = connectionService;
            _pinnedGroupRepository = pinnedGroupRepository;
        }

        public async Task<ChatGroupDto> CreateGroupAsync(
     string name,
     Guid creatorId,
     List<Guid> memberIds)
        {
            var group = await _groupRepo.CreateGroupAsync(
                name,
                creatorId,
                memberIds
            );

            return new ChatGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                CreatedById = group.CreatedById,

                Members = group.Members.Select(m => new ChatGroupMemberDto
                {
                    UserId = m.UserId
                }).ToList()
            };
        }


        public async Task<List<ChatGroupDto>> GetUserGroupsAsync(Guid userId)
        {
            var groups = await _groupRepo.GetUserGroupsAsync(userId);

            var groupIds = groups.Select(g => g.Id).ToList();

            var lastMessages = await _groupMessageRepo
                .GetLastMessagesAsync(groupIds);

            // 🔹 Get pinned groups for user
            var pinnedGroupIds = await _pinnedGroupRepository.GetPinnedGroupIdsAsync(userId);
            var pinnedSet = pinnedGroupIds.ToHashSet();

            return groups
          .Select(g =>
          {
              lastMessages.TryGetValue(g.Id, out var lastMsg);

              return new ChatGroupDto
              {
                  Id = g.Id,
                  Name = g.Name,
                  CreatedById = g.CreatedById,
                  MembersCount = g.Members.Count,
                  Members = g.Members.Select(m => new ChatGroupMemberDto
                  {
                      UserId = m.UserId
                  }).ToList(),
                  LastMessage = lastMsg?.Content,
                  LastMessageTime = lastMsg?.SentAt,
                  LastMessageSenderId = lastMsg?.SenderId,
                  IsPinned = pinnedSet.Contains(g.Id) 
              };
          })
          .OrderByDescending(g => g.IsPinned) // pinned first
          .ThenByDescending(g => g.LastMessageTime ?? DateTime.MinValue) // sort here
          .ToList();

        }


        public async Task<bool> LeaveGroupAsync(Guid groupId, Guid userId)
        {            
            if (groupId == Guid.Empty || userId == Guid.Empty) return false;

            var members = await _groupRepo.GetMemberCountAsync(groupId);
            if (members == 0)
            {
                await _groupRepo.DeleteGroupAsync(groupId);
            }


            return await _groupRepo.RemoveUserFromGroupAsync(groupId, userId);
        }


        public async Task<GroupInfoDto?> GetGroupInfoAsync(Guid groupId, Guid currentUserId)
        {
            if (groupId == Guid.Empty || currentUserId == Guid.Empty)
                return null;

            var group = await _groupRepo.GetGroupInfoAsync(groupId);
            if (group == null)
                return null;

            if (!group.Members.Any(m => m.UserId == currentUserId))
                return null;

            // 1. Fetch usernames from Users table
            var userIds = group.Members.Select(m => m.UserId).ToList();
            var users = await _userRepository.GetAllUsersAsync();
            
            // Convert to dictionary for easy lookup
            var userDict = users?.ToDictionary(u => u.Id, u => u.FullName) ?? new Dictionary<Guid, string>();
            
            // 2. Get total messages count
            var messagesCount = await _groupMessageRepo.GetMessageCountAsync(groupId);

            // 3. Online/offline (replace with your logic)
            var onlineUserIds = _connectionService.GetAllOnlineUserIds(); // e.g., HashSet<Guid>

            return new GroupInfoDto
            {
                Id = group.Id,
                Name = group.Name,
                CreatedById = group.CreatedById,
                CreatedAt = group.CreatedAt,
                MembersCount = group.Members.Count,
                MessagesCount = messagesCount,
                Members = group.Members.Select(m => new GroupMemberInfoDto
                {
                    UserId = m.UserId,
                    Username = userDict.TryGetValue(m.UserId, out var name) ? name : "Unknown",
                    IsAdmin = m.IsAdmin,
                    JoinedAt = m.JoinedAt,
                    IsOnline = onlineUserIds.Contains(m.UserId.ToString())
                }).ToList()
            };
        }

        public async Task<bool> AddMemberAsync(Guid groupId, Guid adminId, Guid userId)
        {
            if (groupId == Guid.Empty || adminId == Guid.Empty || userId == Guid.Empty)
                return false;

            // 1️⃣ Check admin permission
            var isAdmin = await _groupRepo.IsUserAdminAsync(groupId, adminId);
            if (!isAdmin)
                return false;

            // 2️⃣ Check user exists
            var userExists = await _userRepository.ExistsAsync(userId);
            if (!userExists)
                return false;

            // 3️⃣ Add member
            return await _groupRepo.AddMemberAsync(groupId, userId);
        }

    }
}
