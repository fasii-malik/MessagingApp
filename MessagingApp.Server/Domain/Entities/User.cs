using MessagingApp.Server.Domain.Enums;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Server.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [MaxLength(50)]
        public UserRole Role { get; set; } = UserRole.User;
        [Required]
        public DateTime CreatedAt { get; set; }       
        public bool IsBot { get; set; } = false;
        public ICollection<UserPermission> Permissions { get; set; }
            = new List<UserPermission>();
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();

        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    }
}
