using Microsoft.AspNetCore.Identity;

namespace Bug_Ticketing_System
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Project> ManagedProjects { get; set; }
        public virtual ICollection<Bug>? ReportedBugs { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<BugAssignee>? AssignedBugs { get; set; }


    }
}
