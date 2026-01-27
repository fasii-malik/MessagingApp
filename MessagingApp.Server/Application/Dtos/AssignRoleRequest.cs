using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Dtos
{
    public class AssignRoleRequest
    {
        public Guid UserId { get; set; }
        public UserRole Role { get; set; }
    }
}
