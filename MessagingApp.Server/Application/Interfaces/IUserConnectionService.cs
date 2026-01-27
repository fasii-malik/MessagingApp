namespace MessagingApp.Server.Application.Interfaces
{
    public interface IUserConnectionService
    {
        public void Add(string userId, string connectionId);

        public void Remove(string userId, string connectionId);

        public bool IsOnline(string userId);

        public List<string> GetConnectionIds(string userId);
        public List<string> GetAllOnlineUserIds();
    }
}
