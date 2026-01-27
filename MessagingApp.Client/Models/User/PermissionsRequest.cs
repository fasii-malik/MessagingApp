namespace MessagingApp.Client.Models.User
{
    public class PermissionsRequest
    {
        public string UserId { get; set; }

        public List<int> Permissions { get; set; }
    }
}
