namespace MessagingApp.Client.Models.User
{
    public class UserRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool EmailConfirmed { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<int>? Permissions { get; set; } = new();

        public bool IsOnline { get; set; }
    }
}
