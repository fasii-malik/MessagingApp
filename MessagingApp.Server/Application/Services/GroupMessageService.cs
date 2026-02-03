using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Application.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace MessagingApp.Server.Application.Services
{
    public class GroupMessageService : IGroupMessageService
    {
        private readonly IGroupMessageRepository _groupMessageRepository;
        private readonly IMemoryCache _cache;

        public GroupMessageService(IGroupMessageRepository groupMessageRepository, IMemoryCache cache)
        {
            _groupMessageRepository = groupMessageRepository;
            _cache = cache;
        }

        public async Task<GroupMessageDto> SendGroupMessageAsync(Guid groupId, Guid senderId, string content)
        {
            var msg = new GroupMessage
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            // 1️⃣ Save to database
            await _groupMessageRepository.AddAsync(msg);

            // 2️⃣ Map to DTO
            var dto = new GroupMessageDto
            {
                Id = msg.Id,
                GroupId = msg.GroupId,
                SenderId = msg.SenderId,                
                Content = msg.Content,
                SentAt = msg.SentAt
            };

            // 3️⃣ Update in-memory cache for this group
            var cacheKey = $"group:{groupId}";
            List<GroupMessageDto> cachedMessages;

            if (!_cache.TryGetValue(cacheKey, out cachedMessages))
            {
                cachedMessages = new List<GroupMessageDto>();
            }

            cachedMessages.Add(dto);

            // Cache for 1 hour (adjust as needed)
            _cache.Set(cacheKey, cachedMessages, TimeSpan.FromHours(1));

            return dto;
        }

        // 4️⃣ Optional: fetch from cache first
        public async Task<List<GroupMessageDto>> GetGroupMessagesAsync(Guid groupId)
        {
            var cacheKey = $"group:{groupId}";

            if (_cache.TryGetValue(cacheKey, out List<GroupMessageDto>? cachedMessages))
            {
                return cachedMessages!;
            }

            // fallback to DB if cache empty
            var messages = await _groupMessageRepository.GetGroupMessagesAsync(groupId);

            var dtos = messages.Select(m => new GroupMessageDto
            {
                Id = m.Id,
                GroupId = m.GroupId,
                SenderId = m.SenderId,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderName = "" // optionally include sender name if needed
            }).ToList();

            _cache.Set(cacheKey, dtos, TimeSpan.FromHours(1));

            return dtos;
        }

        public async Task<bool> DeleteGroupMessagesAsync(List<Guid> messageIds)
        {
            var groupIds = await _groupMessageRepository.DeleteGroupMessagesAsync(messageIds);

            foreach (var groupId in groupIds.Distinct())
            {
                _cache.Remove($"group:{groupId}");
            }

            return true;
        }

    }

}
