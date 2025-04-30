

using Bug_Ticketing_System.BL.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Bug_Ticketing_System.BL
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResult> RegisterUserAsync(UserAddDto registrationDto)
        {
            var user = new User
            {
                UserName = registrationDto.UserName,
                Email = registrationDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new AuthResult { Success = true };
        }

        public async Task<AuthResult> LoginAsync(UserLoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }

            return new AuthResult { Success = true };
        }
    }
}
