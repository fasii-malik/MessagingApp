using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Interfaces
{
        public interface IUserService
        {
            Task AddAsync(User user);
            Task<User?> GetUserByEmail(string email);
            Task<User?> GetByIdAsync(Guid id);
            Task AssignRoleAsync(Guid userId, UserRole role, UserRole currentUserRole);
            Task UpdateAsync(User user);
            Task DeleteAsync(Guid id);
            Task<IEnumerable<User>?> GetAllUsersAsync();
    }   
}
