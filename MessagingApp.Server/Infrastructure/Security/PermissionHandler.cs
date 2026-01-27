using Microsoft.AspNetCore.Authorization;

namespace MessagingApp.Server.Infrastructure.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll("permission")
                .Select(p => p.Value);

            if (permissions.Contains(requirement.Permission.ToString()))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
