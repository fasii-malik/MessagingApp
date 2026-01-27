using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Dtos
{
    public class AssignPermissionRequest
    {
        public Guid UserId { get; set; }
        public List<Permission> Permissions { get; set; } = new();
    }
}
