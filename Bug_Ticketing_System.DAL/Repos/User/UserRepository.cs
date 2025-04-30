using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bug_Ticketing_System.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly BugTicketingSystemContext _context;
        private readonly UserManager<User> _userManager;
        public IProjectRepository ProjectRepository { get; }

        public UserRepository(
            BugTicketingSystemContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AssignRoleAsync(Guid userId, string roleName)
        {
            var user = await GetUserByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to assign role: {string.Join(", ", result.Errors)}");
            }
        }



        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new KeyNotFoundException($"User with email {email} not found.");
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        public async Task<bool> IsMangerAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, "Manager");
        }


    }
}
