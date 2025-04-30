using Microsoft.EntityFrameworkCore;

namespace Bug_Ticketing_System.DAL
{
    public class BugRepository : IBugRepository
    {
        private readonly BugTicketingSystemContext _context;

        public BugRepository(BugTicketingSystemContext context)
        {
            _context = context;
        }

        public async Task AddBugAsync(Bug bug)
        {
            await _context.Bugs.AddAsync(bug);
        }

        public async Task AssignUserToBugAsync(Guid bugId, Guid userId)
        {
            var bugAssignee = new BugAssignee { BugId = bugId, UserId = userId };
            await _context.BugAssignees.AddAsync(bugAssignee);
        }

        public async Task<Bug> GetBugByIdAsync(Guid bugId)
        {
            return await _context.Bugs
                .Include(b => b.Assignees)
                .ThenInclude(ba=>ba.User)
                .Include(b=>b.Attachments)
                .FirstOrDefaultAsync(b => b.Id == bugId);
        }

        public async Task<List<Bug>> GetBugsAsync()
        {
            return await _context.Bugs
            .Include(b => b.Project)
            .Include(b => b.Reporter)
            .Include(b => b.Assignees!)
            .Include(b => b.Attachments!)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task RemoveUserFromBugAsync(Guid bugId, Guid userId)
        {
            var assignment = await _context.BugAssignees
                .FirstOrDefaultAsync(ba => ba.BugId == bugId && ba.UserId == userId);

            if (assignment != null)
            {
                _context.BugAssignees.Remove(assignment);
            }
        }

        public async Task<BugAssignee> GetAssignmentAsync(Guid bugId, Guid userId)
        {
            return await _context.BugAssignees
                .FirstOrDefaultAsync(ba => ba.BugId == bugId && ba.UserId == userId);
        }


    }
}
