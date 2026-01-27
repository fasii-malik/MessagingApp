using MessagingApp.Server.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace MessagingApp.Server.Infrastructure.Security
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; }

        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }
    }
}
