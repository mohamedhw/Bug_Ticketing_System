using Bug_Ticketing_System.BL.Mangers.Projects;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Bug_Ticketing_System.BL.Mangers.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Bug_Ticketing_System.BL.Validators;
using Microsoft.Extensions.Configuration;
using Bug_Ticketing_System.BL.Dtos;


namespace Bug_Ticketing_System.BL
{
    public static class BusinessExtensions
    {
        public static void AddProject(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProjectManger, ProjectManger>();
            services.AddScoped<IBugManager, BugManager>();
            services.AddScoped<IUserManger, UserManger>();
            services.AddScoped<UserService>();
            services.AddValidatorsFromAssemblyContaining<UserAddDtoValidator>();
            services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

        }
    }
}
