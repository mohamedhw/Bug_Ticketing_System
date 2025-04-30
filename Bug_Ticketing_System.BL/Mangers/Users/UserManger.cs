using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Bug_Ticketing_System.BL.Mangers.Users
{
    public class UserManger : IUserManger
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserAddDto> _validator;

        public UserManger(
            UserManager<User> userManager,
            IValidator<UserAddDto> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }
        public async Task<GeneralResult> AddUserAsync(UserAddDto userAddDto)
        {
            var validationResult = await _validator.ValidateAsync(userAddDto);

            if (!validationResult.IsValid)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new ResultError
                    {
                        Code = e.ErrorCode,
                        Message = e.ErrorMessage
                    }).ToArray()
                };
            }

            var user = new User
            {
                UserName = userAddDto.UserName,
                Email = userAddDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, userAddDto.Password);

            if (!createResult.Succeeded)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = createResult.Errors.Select(e => new ResultError
                    {
                        Code = e.Code,
                        Message = e.Description
                    }).ToArray()
                };
            }

            // Assign roles
            var roleResult = await _userManager.AddToRolesAsync(user, userAddDto.Roles);

            if (!roleResult.Succeeded)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = roleResult.Errors.Select(e => new ResultError
                    {
                        Code = e.Code,
                        Message = e.Description
                    }).ToArray()
                };
            }

            return new GeneralResult { Success = true };
        }

        public async Task<GeneralResult> Login(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = new[] { new ResultError { Code = "InvalidCredentials", Message = "Invalid email or password" } }
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = new[] { new ResultError { Code = "InvalidCredentials", Message = "Invalid email or password" } }
                };
            }

            return new GeneralResult { Success = true };
        }


    }
}
