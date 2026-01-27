using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Server.Application.Dtos
{
    public class UpdateUserRequest
    {
     //   public string UserId { get; set; } = "";
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
