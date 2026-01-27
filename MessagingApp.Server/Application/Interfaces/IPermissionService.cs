using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Interfaces
{
    public interface IPermissionService
    {
        Task AssignAsync(Guid userId, IEnumerable<Permission> permissions);
       // Task<bool> HasPermissionAsync(Guid userId, Permission permission);
        Task RevokeAsync(Guid userId, IEnumerable<Permission> permissions);
    }

}
