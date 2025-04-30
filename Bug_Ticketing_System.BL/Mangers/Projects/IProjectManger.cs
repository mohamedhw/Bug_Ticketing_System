using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;

namespace Bug_Ticketing_System.BL.Mangers.Projects
{
    public interface IProjectManger
    {
        Task<GeneralResult> AddProjectAsync(Dtos.ProjectAddDto projectAddDto);

        Task<ProjectDetailDto> GetProjectByIdAsync(Guid projectId);
        Task<List<ProjectListDto>> GetAllProjectsAsync();

    }
}
