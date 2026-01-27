namespace MessagingApp.Server.Application.Interfaces
{
    using MessagingApp.Server.Application.Dtos;
    using MessagingApp.Server.Domain.Enums;

    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string fullname,string role, List<Permission> permissions, out DateTime expiration);
    }

}
