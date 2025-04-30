

namespace Bug_Ticketing_System
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ManagerId { get; set; }
        public virtual User Manager { get; set; }
        public virtual ICollection<Bug> Bugs { get; set; }

    }
}
