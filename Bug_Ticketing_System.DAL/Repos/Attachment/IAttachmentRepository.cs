namespace Bug_Ticketing_System.DAL
{
    public interface IAttachmentRepository
    {
        void Add(Attachment attachment);
        Task<IEnumerable<Attachment>> GetAttachmentsByBugIdAsync(Guid bugId);
        Task<Attachment> GetAttachmentByIdAsync(Guid attachmentId);
        void Delete(Attachment attachment);
    }
}
