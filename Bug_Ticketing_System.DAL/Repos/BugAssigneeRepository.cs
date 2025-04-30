

namespace Bug_Ticketing_System.DAL.Repos
{
    public class BugAssigneeRepository : IBugAssigneeRepository
    {
        private readonly BugTicketingSystemContext _context;

        public BugAssigneeRepository(BugTicketingSystemContext context)
        {
            _context = context;
        }

    }
}
