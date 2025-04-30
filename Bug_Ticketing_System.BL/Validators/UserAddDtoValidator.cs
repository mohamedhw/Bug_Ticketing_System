using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Bug_Ticketing_System.BL.Dtos;

namespace Bug_Ticketing_System.BL.Validators
{
    public class UserAddDtoValidator : AbstractValidator<UserAddDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserAddDtoValidator(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            RuleFor(d => d.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters");

            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MustAsync(async (email, _) => await IsEmailUnique(email))
                .WithMessage("Email already exists");

            RuleFor(d => d.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(d => d.Roles)
                .Must(roles => roles != null && roles.Any())
                .WithMessage("At least one role is required");

            RuleForEach(d => d.Roles)
                .MustAsync(async (role, _) => await _roleManager.RoleExistsAsync(role))
                .WithMessage("Role '{PropertyValue}' does not exist");
        }

        private async Task<bool> IsEmailUnique(string email)
        {
            return await _userManager.FindByEmailAsync(email) == null;
        }
    }
}