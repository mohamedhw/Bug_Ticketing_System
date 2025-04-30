namespace Bug_Ticketing_System.DAL
{
    public interface IBugRepository
    {
        Task<List<Bug>> GetBugsAsync();
        Task<Bug> GetBugByIdAsync(Guid bugId);
        Task AddBugAsync(Bug bug);
        Task AssignUserToBugAsync(Guid bugId, Guid userId);
        Task RemoveUserFromBugAsync(Guid bugId, Guid userId);
        Task<BugAssignee> GetAssignmentAsync(Guid bugId, Guid userId);

    }
}
