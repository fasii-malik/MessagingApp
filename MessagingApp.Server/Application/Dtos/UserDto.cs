using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool EmailConfirmed { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Permission> Permissions { get; set; } = new();        
        public bool IsOnline { get; set; }
    }
}
