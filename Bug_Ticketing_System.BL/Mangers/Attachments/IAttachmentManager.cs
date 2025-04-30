using Bug_Ticketing_System.BL.Dtos;

namespace Bug_Ticketing_System.BL.Mangers.Attachments
{
    public interface IAttachmentManager
    {
        Task<AttachmentResponseDto> UploadAttachmentAsync(Guid bugId, AttachmentRequestDto request, Guid userId);
        Task<List<AttachmentResponseDto>> GetAttachmentsAsync(Guid bugId);
        Task DeleteAttachmentAsync(Guid bugId, Guid attachmentId, Guid userId);
    }
}
