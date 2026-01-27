using MessagingApp.Server.Application.Interfaces;

namespace MessagingApp.Server.Application.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        // key = userId, value = all connectionIds for this user
        private readonly Dictionary<string, HashSet<string>> _onlineUsers = new();

        private readonly object _lock = new(); // thread-safe

        // Add a connection for a user
        public void Add(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_onlineUsers.ContainsKey(userId))
                    _onlineUsers[userId] = new HashSet<string>();

                _onlineUsers[userId].Add(connectionId);
            }
        }

        // Remove a connection for a user
        public void Remove(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (_onlineUsers.ContainsKey(userId))
                {
                    _onlineUsers[userId].Remove(connectionId);
                    if (_onlineUsers[userId].Count == 0)
                        _onlineUsers.Remove(userId);
                }
            }
        }

        // Check if user is online
        public bool IsOnline(string userId)
        {
            lock (_lock)
            {
                return _onlineUsers.ContainsKey(userId);
            }
        }

        public List<string> GetConnectionIds(string userId)
        {
            lock (_lock)
            {
                return _onlineUsers.ContainsKey(userId) ? _onlineUsers[userId].ToList() : new List<string>();
            }
        }


        // Get all currently online user IDs
        public List<string> GetAllOnlineUserIds()
        {
            lock (_lock)
            {
                return _onlineUsers.Keys.ToList();
            }
        }
    }
}
