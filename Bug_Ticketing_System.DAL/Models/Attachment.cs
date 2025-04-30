

namespace Bug_Ticketing_System
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid BugId { get; set; }
        public virtual Bug Bug { get; set; }
        public Guid UploadedBy { get; set; }
        public virtual User UploadedByUser { get; set; }
    }
}
