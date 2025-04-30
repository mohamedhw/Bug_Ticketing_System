using System.Security.Claims;
using System.Text;
using Bug_Ticketing_System;
using Bug_Ticketing_System.BL;
using Bug_Ticketing_System.BL.Mangers.Attachments;
using Bug_Ticketing_System.BL.Mangers.Projects;
using Bug_Ticketing_System.BL.Mangers.Users;
using Bug_Ticketing_System.DAL;
using Bug_Ticketing_System.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Constants = Bug_Ticketing_System.Constants;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddScoped<ProjectAddDtoValidator>();


builder.Services.AddDataAccessServices(builder.Configuration);


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProjectManger, ProjectManger>();
builder.Services.AddScoped<IAttachmentManager, AttachmentManager>();



builder.Services.AddScoped<UserManger>();
builder.Services.AddProject(builder.Configuration);


#region Identity

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequiredUniqueChars = 2;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<BugTicketingSystemContext>();

#endregion


#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

#endregion


#region Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        Constants.Policies.ForManager,
        builder => builder.RequireClaim(ClaimTypes.Role, "Manager")
    );

    options.AddPolicy(
        Constants.Policies.ForTester,
        builder => builder.RequireClaim(ClaimTypes.Role, "Tester")
    );

    options.AddPolicy(
        Constants.Policies.ForDeveloper,
        builder => builder.RequireClaim(ClaimTypes.Role, "Developer")
    );

    options.AddPolicy(
        Constants.Policies.ForDeveloper,
        builder => builder.RequireClaim(ClaimTypes.Role, "Admin")
);
});
#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 512 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 512 * 1024 * 1024;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");
var webRootPath = builder.Environment.WebRootPath;
if (string.IsNullOrEmpty(webRootPath))
{
    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    Directory.CreateDirectory(webRootPath);
    builder.Environment.WebRootPath = webRootPath;
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(webRootPath, "uploads")),
    RequestPath = "/uploads"
});
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "Manager", "Tester", "Developer", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}
app.Run();
