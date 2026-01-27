using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Server.Application.Dtos
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = default!;
    }
}
