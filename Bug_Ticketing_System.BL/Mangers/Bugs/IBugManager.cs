
using Bug_Ticketing_System.BL.Dtos.Common;
using Bug_Ticketing_System.BL.Dtos;

namespace Bug_Ticketing_System.BL
{
    public interface IBugManager
    {
        Task<List<BugListDto>> GetAllBugsAsync();
        Task<BugDetailDto?> GetBugByIdAsync(Guid bugId);
        Task<GeneralResult> AddBugAsync(BugAddDto bugDto, Guid userId);
        Task AssignUserToBugAsync(Guid bugId, Guid userId);
        Task UnassignUserFromBugAsync(Guid bugId, Guid userId);
    }
}
