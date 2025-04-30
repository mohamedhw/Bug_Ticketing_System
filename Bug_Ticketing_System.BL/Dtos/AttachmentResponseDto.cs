namespace Bug_Ticketing_System.BL.Dtos
{
    public class AttachmentResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UploadedBy { get; set; }
    }
}
