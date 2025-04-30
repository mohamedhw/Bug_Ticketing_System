namespace Bug_Ticketing_System.DAL
{
    public interface IProjectRepository
    {
        Task AddProjectAsync(Project project);
        Task<Project> GetProjectDetailAsync(Guid id);
        Task<List<Project>> GetAllProjectsAsync();


    }
}
