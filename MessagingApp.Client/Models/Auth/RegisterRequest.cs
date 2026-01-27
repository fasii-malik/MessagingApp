using System.ComponentModel.DataAnnotations;

namespace MessagingApp.Client.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
