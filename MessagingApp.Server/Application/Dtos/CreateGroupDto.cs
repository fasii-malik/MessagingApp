namespace MessagingApp.Server.Application.Dtos
{
    public class CreateGroupDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<Guid> MemberIds { get; set; } = new();
    }

}
