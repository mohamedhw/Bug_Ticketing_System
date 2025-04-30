

namespace Bug_Ticketing_System
{
    public class BugAssignee
    {
        public Guid BugId { get; set; }
        public Guid UserId { get; set; }
        public Bug Bug { get; set; }
        public User User { get; set; }
    }
}
