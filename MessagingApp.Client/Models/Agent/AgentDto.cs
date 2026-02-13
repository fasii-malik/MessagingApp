namespace MessagingApp.Client.Models.Agent
{
    public class AgentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOnline { get; set; } // optional
    }

}
