using MessagingApp.Server.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Server.Domain.Entities
{
    public class UserPermission
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Permission Permission { get; set; } 
    }
}
