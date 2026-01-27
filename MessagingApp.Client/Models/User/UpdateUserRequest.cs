using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Client.Models.User
{
    public class UpdateUserRequest
    {
        public Guid userId { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
