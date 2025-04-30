using System.Security.Claims;
using Bug_Ticketing_System.BL;
using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bug_Ticketing_System.Controllers
{
    [Route("api/bugs/")]
    [ApiController]
    public class BugController : ControllerBase
    {
        private readonly IBugManager _bugManager;
        public BugController(IBugManager bugManager)
        {
            _bugManager = bugManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<BugListDto>>> GetAll()
        {
            var bugs = await _bugManager.GetAllBugsAsync();
            return Ok(bugs);
        }

        [HttpGet("{bugId}")]
        public async Task<ActionResult<BugDetailDto>> GetBug(Guid bugId)
        {
            var bug = await _bugManager.GetBugByIdAsync(bugId);
            return bug == null ? NotFound() : Ok(bug);
        }


        [HttpPost]
        [Authorize(Policy = Constants.Policies.ForTester)]
        public async Task<ActionResult<GeneralResult>> AddBug(BugAddDto bugDto)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(reporterId, out Guid reporterIdGuid))
            {
                return BadRequest(new GeneralResult
                {
                    Success = false,
                    Errors = new[] { new ResultError { Code = "InvalidUser", Message = "Invalid user identifier" } }
                });
            }
            var result = await _bugManager.AddBugAsync(bugDto, reporterIdGuid);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("{bugId}/assignees")]
        [Authorize(Policy = Constants.Policies.ForManager)]
        public async Task<IActionResult> AssignBugToUser(Guid bugId, [FromBody] AssignUserToBugDto dto)
        {

            var user = User;
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
            var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            var roles = user.Claims.Where(c => c.Type.EndsWith("role")).Select(c => c.Value).ToList();


            try
            {
                await _bugManager.AssignUserToBugAsync(bugId, dto.UserId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return ex.Message.Contains("must be a Developer")
                ? BadRequest(ex.Message)
                : NotFound(ex.Message);
                return NotFound(ex.Message);
            }
        }



        [HttpDelete("{bugId}/assignees/{userId}")]
        [Authorize(Policy = Constants.Policies.ForManager)]
        public async Task<IActionResult> UnassignUserFromBug(
            Guid bugId,
            Guid userId)
        {
            try
            {
                await _bugManager.UnassignUserFromBugAsync(bugId, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
