using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;
using Bug_Ticketing_System.DAL;
using Bug_Ticketing_System.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace Bug_Ticketing_System.BL
{

    public class BugManager : IBugManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BugAddDtoValidator _validator;
        private readonly IBugRepository _bugRepository;
        private readonly UserManager<User> _userManager;

        public BugManager(
            IUnitOfWork unitOfWork,
            BugAddDtoValidator validator,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userManager = userManager;
        }

        public async Task<List<BugListDto>> GetAllBugsAsync()
        {
            var bugs = await _unitOfWork.BugRepository.GetBugsAsync();

            return bugs.Select(b => new BugListDto
            {
                Id = b.Id,
                Title = b.Title,
                Status = b.Status,
                Priority = b.Priority,
                CreatedAt = b.CreatedAt,
                ProjectName = b.Project?.ProjectName,
                ReporterName = b.Reporter?.UserName,
                AssigneeCount = b.Assignees?.Count ?? 0,
                AttachmentCount = b.Attachments?.Count ?? 0
            }).ToList();
        }

        public async Task<BugDetailDto?> GetBugByIdAsync(Guid bugId)
        {
            var bug = await _unitOfWork.BugRepository.GetBugByIdAsync(bugId);
            if (bug == null) return null;

            return new BugDetailDto
            {
                Id = bug.Id,
                Title = bug.Title,
                Description = bug.Description,
                Status = bug.Status,
                Priority = bug.Priority,
                CreatedAt = bug.CreatedAt,
                LastUpdate = bug.LastUpdate,
                ProjectId = bug.ProjectId,
                ProjectName = bug.Project?.ProjectName,
                ReporterId = bug.ReporterId,
                ReporterName = bug.Reporter?.UserName,
                Assignees = bug.Assignees?.Select(a => new UserDto
                {
                    UserId = a.UserId,
                    UserName = a.User?.UserName,
                    Email = a.User?.Email
                }).ToList() ?? new List<UserDto>(),
                Attachments = bug.Attachments?.Select(a => new AttachmentDto
                {
                    AttachmentId = a.Id,
                    FileName = a.FileName,
                    CreatedAt = a.CreatedAt
                }).ToList() ?? new List<AttachmentDto>()
            };
        }

        public async Task<GeneralResult> AddBugAsync(BugAddDto bugDto, Guid userId)
        {
            var validationResult = await _validator.ValidateAsync(bugDto);
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

            var bug = new Bug
            {
                Title = bugDto.Title,
                Description = bugDto.Description,
                Status = BugStatus.Open.ToString(),
                Priority = bugDto.Priority,
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                ProjectId = bugDto.ProjectId,
                ReporterId = userId
            };

            try
            {
                await _unitOfWork.BugRepository.AddBugAsync(bug);
                await _unitOfWork.SaveChangesAsync();

                return new GeneralResult { Success = true };
            }
            catch (Exception ex)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = [new ResultError { Code = "500", Message = ex.Message }]
                };
            }
        }

        public async Task AssignUserToBugAsync(Guid bugId, Guid userId)
        {

            var bug = await _unitOfWork.BugRepository.GetBugByIdAsync(bugId)
                ?? throw new ArgumentException("Bug not found");

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new ArgumentException("User not found");

            if (!await _userManager.IsInRoleAsync(user, "Developer"))
            {
                throw new ArgumentException("User must be a Developer to be assigned to a bug");
            }

            var existing = await _unitOfWork.BugRepository.GetAssignmentAsync(bugId, userId);
            if (existing != null)
                throw new ArgumentException("User already assigned to this bug");

            await _unitOfWork.BugRepository.AssignUserToBugAsync(bugId, userId);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnassignUserFromBugAsync(Guid bugId, Guid userId)
        {
            var assignment = await _unitOfWork.BugRepository.GetAssignmentAsync(bugId, userId)
                ?? throw new ArgumentException("Assignment not found");

            await _unitOfWork.BugRepository.RemoveUserFromBugAsync(bugId, userId);

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
