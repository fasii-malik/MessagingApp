using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;
using MessagingApp.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (users == null)
                throw new ApplicationException("Failed to retrieve users");

            return users;
        }


        public async Task AddAsync(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));
            
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user id");

            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                throw new ArgumentException("Invalid user id");

            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Invalid user email");

            email = email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
                throw new ArgumentException("Invalid user id");

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (user.Id == Guid.Empty)
                throw new ArgumentException("Invalid User Id", nameof(user.Id));

            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser is null)
                throw new KeyNotFoundException("User not found");
            
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            //existingUser.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(existingUser);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user id", nameof(id));

            var existingUser = await _userRepository.GetByIdAsync(id);

            if (existingUser is null)
                throw new KeyNotFoundException("User not found");

            _userRepository.Remove(existingUser);
            await _userRepository.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(Guid userId, UserRole role, UserRole currentUserRole)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user id");

            if (currentUserRole == UserRole.Admin && role == UserRole.SuperAdmin)
                throw new UnauthorizedAccessException("Admin cannot assign SuperAdmin role");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Role = role;

            await _userRepository.SaveChangesAsync();
        }
    }//class
}//namespace
