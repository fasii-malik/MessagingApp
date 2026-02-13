using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MessagingApp.Server.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepo;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IPinnedChatRepository _pinnedChatRepo;

        public MessageService(IMessageRepository repo, IMemoryCache cache, IUserRepository userRepo, IUserConnectionService userConnectionService, IPinnedChatRepository pinnedChatRepo)
        {
            _repo = repo;
            _cache = cache;
            _userRepo = userRepo;
            _userConnectionService = userConnectionService;
            _pinnedChatRepo = pinnedChatRepo;
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.Parse(senderId),
                ReceiverId = Guid.Parse(receiverId),
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            // Save to DB
            await _repo.AddAsync(message);

            _cache.Remove(GetCacheKey(senderId, receiverId));

            //// Update cache
            //var key = GetCacheKey(senderId, receiverId);
            //if (!_cache.TryGetValue(key, out List<Message> messages))
            //{
            //    messages = new List<Message>();
            //}
            //messages.Add(message);
            //_cache.Set(key, messages, TimeSpan.FromMinutes(5));

            return message;
        }

        public async Task<List<Message>> GetChatAsync(string userAId, string userBId)
        {
            var key = GetCacheKey(userAId, userBId);

            if (!_cache.TryGetValue(key, out List<Message> messages))
            {
                messages = await _repo.GetChatAsync(userAId, userBId);
                _cache.Set(key, messages, TimeSpan.FromMinutes(5));
            }

            return messages;
        }

        private string GetCacheKey(string userA, string userB)
        {
            var ids = new[] { userA, userB }.OrderBy(x => x).ToArray();
            return $"chat:{ids[0]}:{ids[1]}";
        }

        public async Task<List<UserDto>> GetAllUsersAsync(string currentUserId)
        {
            // Fetch all users from your database
            var allUsers = await _userRepo.GetAllUsersAsync(); // replace with your actual method            

            // Get all online user IDs from the connection service
            var onlineIds = _userConnectionService.GetAllOnlineUserIds();

            // Map to DTOs, excluding the current user     .Where(u => u.Id.ToString() != currentUserId)
            var usersDto = allUsers                
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    IsOnline = onlineIds.Contains(u.Id.ToString())
                })
                .ToList();

            return usersDto;
        }


        public async Task<List<ConversationDto>> GetConversationsAsync(string currentUserId)
        {
            var currentUserGuid = Guid.Parse(currentUserId);

            // 1️⃣ Get pinned user ids
            var pinnedUserIds = await _pinnedChatRepo.GetPinnedUserIdsAsync(currentUserGuid);

            // 1️⃣ Get last message per conversation directly from DB
            var lastMessages = await _repo.GetAllMessagesForUserAsync(currentUserId);

            var conversations = lastMessages
                .GroupBy(m => m.SenderId == currentUserGuid ? m.ReceiverId : m.SenderId) // group by other user
                .Select(g =>
                {
                    var lastMsg = g.OrderByDescending(m => m.CreatedAt).First();

                    return new
                    {
                        OtherUserId = g.Key,
                        LastMessage = lastMsg.Content,
                        LastMessageTime = lastMsg.CreatedAt,
                        IsPinned = pinnedUserIds.Contains(g.Key)
                    };
                })
                .OrderByDescending(c => c.IsPinned)
                .ThenByDescending(c => c.LastMessageTime)
                .ToList();

            // 2️⃣ Map to DTO
            var conversationDtos = new List<ConversationDto>();
            foreach (var convo in conversations)
            {
                var user = await _userRepo.GetByIdAsync(convo.OtherUserId);
                if (user == null) continue;

                conversationDtos.Add(new ConversationDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    LastMessage = convo.LastMessage,
                    LastMessageTime = convo.LastMessageTime,
                    IsPinned = convo.IsPinned,
                    IsOnline = _userConnectionService.IsOnline(user.Id.ToString())
                });
            }

            return conversationDtos;
        }


        public async Task DeleteMessagesAsync(IEnumerable<Guid> messageIds, Guid currentUserId)
        {
            if (messageIds == null || !messageIds.Any())
                return;
            
            // 🔥 Load messages FIRST
            var messages = await _repo.GetAllMessagesForUserAsync(currentUserId.ToString());

            await _repo.DeleteManyAsync(messageIds);

            // 🔥 Invalidate BOTH cache directions
            foreach (var msg in messages)
            {
                var key1 = GetCacheKey(msg.SenderId.ToString(), msg.ReceiverId.ToString());
                var key2 = GetCacheKey(msg.ReceiverId.ToString(), msg.SenderId.ToString());

                _cache.Remove(key1);
                _cache.Remove(key2);
            }
        }

        public async Task DeleteConversationAsync(Guid currentUserId, Guid otherUserId)
        {
            if (currentUserId == Guid.Empty || otherUserId == Guid.Empty)
                return;
            // Call repository to delete all messages between these two users
            await _repo.DeleteConversationAsync(currentUserId, otherUserId);

        }

    }
}
