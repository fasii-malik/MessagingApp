using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Server.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.Permission });

            // Configure relationship
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure Sent Messages relationship
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Important: Prevents cycles

            // Configure Received Messages relationship
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Important: Prevents cycles

            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed SuperAdmin user
            var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = superAdminId,
                FullName = "Super Admin",
                Email = "superadmin@messagingapp.com",
                EmailConfirmed = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@1234"), 
                Role = UserRole.SuperAdmin,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Optional: Seed permissions for SuperAdmin
            modelBuilder.Entity<UserPermission>().HasData(
                new UserPermission
                {
                    UserId = superAdminId,
                    Permission = Permission.AssignPermissions
                },
                new UserPermission
                {
                    UserId = superAdminId,
                    Permission = Permission.ViewDashboard
                },
                new UserPermission
                {
                    UserId = superAdminId,
                    Permission = Permission.ManageUsers
                },
                new UserPermission
                {
                    UserId = superAdminId,
                    Permission = Permission.AssignRoles
                }
            );
        }
    }
}
