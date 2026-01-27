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

        public MessageService(IMessageRepository repo, IMemoryCache cache, IUserRepository userRepo, IUserConnectionService userConnectionService)
        {
            _repo = repo;
            _cache = cache;
            _userRepo = userRepo;
            _userConnectionService = userConnectionService;
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

            // Update cache
            var key = GetCacheKey(senderId, receiverId);
            if (!_cache.TryGetValue(key, out List<Message> messages))
            {
                messages = new List<Message>();
            }
            messages.Add(message);
            _cache.Set(key, messages, TimeSpan.FromMinutes(5));

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
            // 1️⃣ Get all messages involving current user
            var messages = await _repo.GetAllMessagesForUserAsync(currentUserId);

            // 2️⃣ Extract distinct other users
            var otherUserIds = messages
                .Select(m =>
                    m.SenderId.ToString() == currentUserId
                        ? m.ReceiverId.ToString()
                        : m.SenderId.ToString())
                .Distinct()
                .ToList();

            var conversations = new List<ConversationDto>();

            // 3️⃣ For each user → get last message
            foreach (var otherUserId in otherUserIds)
            {
                var lastMessage = await _repo.GetLastMessageAsync(currentUserId, otherUserId);
                var user = await _userRepo.GetByIdAsync(Guid.Parse(otherUserId));

                if (lastMessage == null || user == null) continue;

                conversations.Add(new ConversationDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    LastMessage = lastMessage.Content,
                    LastMessageTime = lastMessage.CreatedAt,
                    IsOnline = _userConnectionService.IsOnline(user.Id.ToString())
                });
            }

            // 4️⃣ Sort like WhatsApp
            return conversations
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();
        }

    }
}
