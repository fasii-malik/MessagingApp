using MessagingApp.Server.Domain.Enums;

namespace MessagingApp.Server.Application.Security
{
    public class RolePermissionMap
    {
        public static readonly Dictionary<UserRole, Permission[]> Map =
        new()
        {
            //[UserRole.User] = new[]
            //{
            //    Permission.CreatePost
            //},

            [UserRole.Admin] = new[]
            {
                Permission.ViewDashboard,
                Permission.ManageUsers,
            //   Permission.AssignPermissions
            },

            [UserRole.SuperAdmin] = Enum.GetValues<Permission>()
        };
    }
}
