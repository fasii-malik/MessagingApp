using MessagingApp.Server.Application.Services;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Repositories;
using MessagingApp.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> AddAsync(User user)
        {            
            ArgumentNullException.ThrowIfNull(user);
            
            // Use .Add(user) for most cases; it returns EntityEntry<User>
            var entry = await _context.Users.AddAsync(user);

            // Return the tracked entity
            return entry.Entity;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(p => p.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.Include(p => p.Permissions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }


            public async Task<IEnumerable<User>?> GetAllUsersAsync()
            {
                var users = await _context.Users
                    .Include(p => p.Permissions)
                    .AsNoTracking()
                    .ToListAsync();

                // Log number of users retrieved
                _logger.LogInformation("GetAllUsersAsync returned {Count} users", users.Count);

                // Optional: log details of first few users
                foreach (var u in users.Take(5)) // only first 5 to avoid huge logs
                {
                    var permNames = u.Permissions.Select(p => p.Permission).ToList();
                    _logger.LogInformation("User: {Name}, Permissions: {@Permissions}", u.FullName, permNames);
                }

                return users;
            }
        }
}
