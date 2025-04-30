using Bug_Ticketing_System.DAL.UnitOfWork;
using Bug_Ticketing_System.DAL;
using Microsoft.AspNetCore.Hosting;
using Bug_Ticketing_System.BL.Dtos;

namespace Bug_Ticketing_System.BL.Mangers.Attachments
{
    public class AttachmentManager : IAttachmentManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBugRepository _bugRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IWebHostEnvironment _environment;

        public AttachmentManager(
            IUnitOfWork unitOfWork,
            IBugRepository bugRepository,
            IAttachmentRepository attachmentRepository,
            IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _bugRepository = bugRepository;
            _attachmentRepository = attachmentRepository;
            _environment = environment;
        }

        public async Task<AttachmentResponseDto> UploadAttachmentAsync(Guid bugId, AttachmentRequestDto request, Guid userId)
        {
            var bug = await _bugRepository.GetBugByIdAsync(bugId);
            if (bug == null) throw new ArgumentException("Bug not found.");

            Console.WriteLine($"File received: {request.File?.FileName}, Length: {request.File?.Length}");

            var file = request.File;

            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded ");

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                CreatedAt = DateTime.UtcNow,
                BugId = bugId,
                UploadedBy = userId
            };

            _attachmentRepository.Add(attachment);
            await _unitOfWork.SaveChangesAsync();

            return new AttachmentResponseDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                CreatedAt = attachment.CreatedAt,
                UploadedBy = attachment.UploadedBy
            };
        }

        public async Task<List<AttachmentResponseDto>> GetAttachmentsAsync(Guid bugId)
        {
            var attachments = await _attachmentRepository.GetAttachmentsByBugIdAsync(bugId);
            return attachments.Select(a => new AttachmentResponseDto
            {
                Id = a.Id,
                FileName = a.FileName,
                CreatedAt = a.CreatedAt,
                UploadedBy = a.UploadedBy
            }).ToList();
        }

        public async Task DeleteAttachmentAsync(Guid bugId, Guid attachmentId, Guid userId)
        {


            var attachment = await _attachmentRepository.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null || attachment.BugId != bugId)
                throw new ArgumentException("Attachment not found.");
            if (attachment.UploadedBy != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this attachment.");

            if (System.IO.File.Exists(attachment.FilePath))
                System.IO.File.Delete(attachment.FilePath);

            _attachmentRepository.Delete(attachment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
