using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Mangers.Users;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Bug_Ticketing_System.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IValidator<UserAddDto> _validator;
        private readonly IValidator<UserLoginDto> _loginValidator;
        private readonly IConfiguration _configuration;
        private readonly IUserManger _userManger;
        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            IValidator<UserLoginDto> loginValidator,
            IUserManger userManger,
            IValidator<UserAddDto> validator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _validator = validator;
            _loginValidator = loginValidator;
            _userManger = userManger;
        }


        private TokenDto GenerateToken(IEnumerable<Claim> claims)
        {
            var secretKey = _configuration["Jwt:SecretKey"];


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var expires = DateTime.UtcNow.AddHours(
                _configuration.GetValue<int>("Jwt:ExpiryHours", 24)
            );

            var token = new JwtSecurityToken(

                claims: claims,
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new TokenDto(
                new JwtSecurityTokenHandler().WriteToken(token),
                token.ValidTo
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAddDto userDto)
        {
            var validationResult = await _validator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, userDto.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(e => e.Description));
            }
            var roleResult = await _userManager.AddToRolesAsync(user, userDto.Roles);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(roleResult.Errors.Select(e => e.Description));
            }

            // Generate JWT
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role =>
                new Claim(ClaimTypes.Role, role)
            ));
            var token = GenerateToken(claims);
            return Ok(token);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _userManger.Login(loginDto);
            if (!result.Success)
            {
                return Unauthorized(result.Errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add real roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = GenerateToken(claims);
            return Ok(token);
        }
    }
}
