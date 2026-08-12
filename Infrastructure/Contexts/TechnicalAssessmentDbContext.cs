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

            // =========================
            // Identity Tables
            // =========================

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens");
            });

            // =========================
            // Fixed IDs
            // =========================

            const string adminRoleId =
                "11111111-1111-1111-1111-111111111111";

            const string supportAgentRoleId =
                "22222222-2222-2222-2222-222222222222";

            const string customerRoleId =
                "33333333-3333-3333-3333-333333333333";


            const string adminUserId =
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            const string supportAgentUserId =
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

            const string customerUserId =
                "cccccccc-cccc-cccc-cccc-cccccccccccc";


            // =========================
            // Roles
            // =========================

            modelBuilder.Entity<IdentityRole>().HasData(

                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "admin-role-concurrency"
                },

                new IdentityRole
                {
                    Id = supportAgentRoleId,
                    Name = "SupportAgent",
                    NormalizedName = "SUPPORTAGENT",
                    ConcurrencyStamp = "support-agent-role-concurrency"
                },

                new IdentityRole
                {
                    Id = customerRoleId,
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = "customer-role-concurrency"
                }
            );


            // =========================
            // Password Hasher
            // =========================

            var passwordHasher = new PasswordHasher<User>();


            // =========================
            // Admin User
            // =========================

            var adminUser = new User
            {
                Id = adminUserId,

                UserName = "admin",
                NormalizedUserName = "ADMIN",

                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",

                EmailConfirmed = true,

                PasswordHash = passwordHasher.HashPassword(
                    null!,
                    "Admin@1234"
                ),

                SecurityStamp = "admin-security-stamp",
                ConcurrencyStamp = "admin-concurrency-stamp",

                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };


            // =========================
            // Support Agent User
            // =========================

            var supportAgentUser = new User
            {
                Id = supportAgentUserId,

                UserName = "supportagent",
                NormalizedUserName = "SUPPORTAGENT",

                Email = "supportagent@example.com",
                NormalizedEmail = "SUPPORTAGENT@EXAMPLE.COM",

                EmailConfirmed = true,

                PasswordHash = passwordHasher.HashPassword(
                    null!,
                    "Agent@1234"
                ),

                SecurityStamp = "support-agent-security-stamp",
                ConcurrencyStamp = "support-agent-concurrency-stamp",

                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };


            // =========================
            // Customer User
            // =========================

            var customerUser = new User
            {
                Id = customerUserId,

                UserName = "customer",
                NormalizedUserName = "CUSTOMER",

                Email = "customer@example.com",
                NormalizedEmail = "CUSTOMER@EXAMPLE.COM",

                EmailConfirmed = true,

                PasswordHash = passwordHasher.HashPassword(
                    null!,
                    "Customer@1234"
                ),

                SecurityStamp = "customer-security-stamp",
                ConcurrencyStamp = "customer-concurrency-stamp",

                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };


            // =========================
            // Users
            // =========================

            modelBuilder.Entity<User>().HasData(
                adminUser,
                supportAgentUser,
                customerUser
            );


            // =========================
            // User Roles
            // =========================

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(

                new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                },

                new IdentityUserRole<string>
                {
                    UserId = supportAgentUserId,
                    RoleId = supportAgentRoleId
                },

                new IdentityUserRole<string>
                {
                    UserId = customerUserId,
                    RoleId = customerRoleId
                }
            );


            // =========================
            // Apply Entity Configurations
            // =========================

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TechnicalAssessmentDbContext).Assembly
            );
        }

    }
}
