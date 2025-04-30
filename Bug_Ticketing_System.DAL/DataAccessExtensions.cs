using Bug_Ticketing_System.DAL.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bug_Ticketing_System.DAL
{
    public static class DataAccessExtensions
    {
        public static void AddDataAccessServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<BugTicketingSystemContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<BugTicketingSystemContext>()
                .AddDefaultTokenProviders();

            SeedInitialData(services);

            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IBugAssigneeRepository, BugAssigneeRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBugRepository, BugRepository>();
        }

        private static void SeedInitialData(IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var serviceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
                using var scope = serviceScopeFactory.CreateScope();
                SeedRolesAsync(scope.ServiceProvider).Wait();
                SeedInitialUserAsync(scope.ServiceProvider).Wait();
                return Task.CompletedTask;
            });
        }

        private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            string[] roles = { "Manager", "Developer", "Tester" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        private static async Task SeedInitialUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            const string testEmail = "email@email.com";

            if (await userManager.FindByEmailAsync(testEmail) == null)
            {
                var user = new User
                {
                    UserName = "User1",
                    Email = testEmail,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await userManager.CreateAsync(user, "P@ssw0rd!");
                await userManager.AddToRoleAsync(user, "Manager");
            }
        }
    }
}