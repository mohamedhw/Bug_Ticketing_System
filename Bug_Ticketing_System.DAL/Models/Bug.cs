

namespace Bug_Ticketing_System
{
    public enum BugStatus { Open, InProgress, Resolved, Closed }
    public enum PriorityLevel { Low, Medium, High, Critical }
    public class Bug
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }

        public Guid ProjectId { get; set; }
        public Guid ReporterId { get; set; }

        public Project Project { get; set; }
        public User Reporter { get; set; }

        public List<BugAssignee>? Assignees { get; set; }
        public List<Attachment>? Attachments { get; set; }

    }
}
