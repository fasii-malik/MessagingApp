using MessagingApp.Server.Domain.Entities;

namespace MessagingApp.Server.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);

        Task<User> AddAsync(User user);

        void Update(User user);

        Task SaveChangesAsync();

        void Remove(User user);

        Task<IEnumerable<User>?> GetAllUsersAsync();

        Task<bool> ExistsAsync(Guid userId);
    }
}
