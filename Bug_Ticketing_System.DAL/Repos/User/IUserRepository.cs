namespace Bug_Ticketing_System.DAL
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task AssignRoleAsync(Guid userId, string roleName);
        public IProjectRepository ProjectRepository { get; }

        Task<bool> IsMangerAsync(Guid userId);

    }
}
