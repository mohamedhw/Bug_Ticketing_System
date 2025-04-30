using Microsoft.EntityFrameworkCore;

namespace Bug_Ticketing_System.DAL
{

    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly BugTicketingSystemContext _context;

        public AttachmentRepository(BugTicketingSystemContext context)
        {
            _context = context;
        }

        public void Add(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsByBugIdAsync(Guid bugId)
        {
            return await _context.Attachments
                .Where(a => a.BugId == bugId)
                .Include(a => a.UploadedByUser)
                .ToListAsync();
        }

        public async Task<Attachment> GetAttachmentByIdAsync(Guid attachmentId)
        {
            return await _context.Attachments
                .Include(a => a.UploadedByUser)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }

        public void Delete(Attachment attachment)
        {
            _context.Attachments.Remove(attachment);
        }
    }
}
