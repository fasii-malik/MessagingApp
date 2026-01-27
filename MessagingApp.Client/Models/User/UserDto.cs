namespace MessagingApp.Client.Models.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public bool IsOnline { get; set; }
    }

}
