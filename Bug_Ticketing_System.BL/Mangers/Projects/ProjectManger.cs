using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;
using Bug_Ticketing_System.DAL.UnitOfWork;

namespace Bug_Ticketing_System.BL.Mangers.Projects
{
    public class ProjectManger : IProjectManger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProjectAddDtoValidator _addValidator;

        public ProjectManger(IUnitOfWork unitOfWork, ProjectAddDtoValidator addValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
        }

        public async Task<GeneralResult> AddProjectAsync(ProjectAddDto projectAddDto)
        {

            try
            {
                var isManager = await _unitOfWork.UserRepository.IsMangerAsync(projectAddDto.ManagerId);
                if (!isManager)
                {
                    return new GeneralResult { Success = false, Errors = [new ResultError { Code = "403", Message = "User must be a Manager" }] };
                }

                var validationResult = await  _addValidator.ValidateAsync(projectAddDto);

                if (!validationResult.IsValid) {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = validationResult.Errors
                        .Select(e => new ResultError
                        {
                            Code = e.ErrorCode,
                            Message = e.ErrorMessage
                        })
                        .ToArray()
                    };
                }
                var project = new Project
                {
                    ProjectName = projectAddDto.ProjectName,
                    Description = projectAddDto.Description,
                    StartDate = projectAddDto.StartDate,
                    EndDate = projectAddDto.EndDate,
                    ManagerId = projectAddDto.ManagerId
                };

                await _unitOfWork.ProjectRepository.AddProjectAsync(project);
                await _unitOfWork.SaveChangesAsync();

                return new GeneralResult
                {
                    Success = true,
                    Errors = []
                };

            }
            catch (KeyNotFoundException ex)
            {
                return new GeneralResult { Success = false, Errors = [new ResultError { Code = "404", Message = ex.Message }] };
            }
        }

         public async Task<ProjectDetailDto> GetProjectByIdAsync(Guid projectId)
            {
                var project = await _unitOfWork.ProjectRepository.GetProjectDetailAsync(projectId);
    
                if (project == null) return null;

                return new ProjectDetailDto
                {
                    Id = project.ProjectId,
                    Name = project.ProjectName,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ManagerId = project.ManagerId,
                    ManagerName = project.Manager?.UserName,
                    BugIds = project.Bugs?.Select(b => b.Id).ToList() ?? new List<Guid>()
                };
         }

        public async Task<List<ProjectListDto>> GetAllProjectsAsync()
        {
            var projects = await _unitOfWork.ProjectRepository.GetAllProjectsAsync();

            return projects.Select(p => new ProjectListDto
            {
                Id = p.ProjectId,
                Name = p.ProjectName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ManagerName = p.Manager?.UserName ?? "No Manager",
                BugCount = p.Bugs?.Count ?? 0
            }).ToList();
        }
    }
}
