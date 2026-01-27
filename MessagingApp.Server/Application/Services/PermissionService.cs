using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Security;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;
using MessagingApp.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MessagingApp.Server.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserRepository _userRepository;

        public PermissionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AssignAsync(Guid userId, IEnumerable<Permission> permissions)
        {
            var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException();

 //           user.Permissions.Clear();

            foreach (var permission in permissions)
            {
                if (!user.Permissions.Any(p => p.Permission == permission))
                {
                    user.Permissions.Add(new UserPermission
                    {
                        UserId = userId,
                        Permission = permission
                    });
                }
            }

            await _userRepository.SaveChangesAsync();
        }
        
        public async Task RevokeAsync(Guid userId, IEnumerable<Permission> permissions)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found");

            var permissionsToRemove = user.Permissions
                .Where(p => permissions.Contains(p.Permission))
                .ToList();

            foreach (var permission in permissionsToRemove)
            {
                user.Permissions.Remove(permission);
            }

            await _userRepository.SaveChangesAsync();
        }

        //public async Task<bool> HasPermissionAsync(Guid userId, Permission permission)
        //{
        //    var user = await _userRepository.GetByIdAsync(userId);
        //    if (user is null)
        //        return false;

        //    var rolePermissions = RolePermissionMap.Map[user.Role];

        //    return rolePermissions.Contains(permission) ||
        //           user.Permissions.Any(p => p.Permission == permission);
        //}
    }
}
