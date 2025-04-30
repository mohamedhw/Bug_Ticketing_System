using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;
using Bug_Ticketing_System.BL.Mangers.Projects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bug_Ticketing_System.Controllers
{
    [ApiController]
    [Route("api/Projects/")]
    public class ProjectController : Controller
    {
        private readonly IProjectManger _projectManager;


        public ProjectController(IProjectManger projectManager)
        {
            _projectManager = projectManager;
        }


        [HttpPost]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> Add(ProjectAddDto project)
        {
            var result = await _projectManager.AddProjectAsync(project);
            if (result.Success)
            {
                return TypedResults.Ok(result);
            }
            return TypedResults.BadRequest(result);
        }


        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDetailDto>> GetById(Guid projectId)
        {
            var project = await _projectManager.GetProjectByIdAsync(projectId);
            return project == null ? NotFound() : Ok(project);
        }


        [HttpGet]
        public async Task<ActionResult<List<ProjectListDto>>> GetAll()
        {
            var projects = await _projectManager.GetAllProjectsAsync();
            return Ok(projects);
        }
    }
}
