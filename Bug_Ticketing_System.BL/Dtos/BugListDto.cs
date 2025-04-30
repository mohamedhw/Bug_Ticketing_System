

namespace Bug_Ticketing_System.BL.Dtos
{
    public class BugListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectName { get; set; }
        public string ReporterName { get; set; }
        public int AssigneeCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
