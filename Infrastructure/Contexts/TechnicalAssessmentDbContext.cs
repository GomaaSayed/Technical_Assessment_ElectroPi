using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace Technical_Assessment_ElectroPi.Infrastructure.Contexts
{
    public class TechnicalAssessmentDbContext : IdentityDbContext<User>
    {
        public DbSet<Ticket> Tickets => Set<Ticket>();

        public DbSet<TicketComment> TicketComments => Set<TicketComment>();

        public DbSet<TicketActivity> TicketActivities => Set<TicketActivity>();

        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public TechnicalAssessmentDbContext(DbContextOptions<TechnicalAssessmentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>(b => { b.ToTable("Users"); });
            modelBuilder.Entity<IdentityRole>(b => { b.ToTable("Roles"); });
            modelBuilder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaims"); });
            modelBuilder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserTokens"); });
            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@1234"),
                    SecurityStamp = "admin_security_stamp",
                    ConcurrencyStamp = "admin_concurrency_stamp",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "User@1234"),
                    SecurityStamp = "user_security_stamp",
                    ConcurrencyStamp = "user_concurrency_stamp",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                }
            );

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TechnicalAssessmentDbContext).Assembly);
        }

    }
}
