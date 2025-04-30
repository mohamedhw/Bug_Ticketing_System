

namespace Bug_Ticketing_System.BL.Dtos
{
    public class BugDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }

        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }

        public Guid ReporterId { get; set; }
        public string ReporterName { get; set; }

        public List<UserDto> Assignees { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
    }
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class AttachmentDto
    {
        public Guid AttachmentId { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
