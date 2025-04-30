using Microsoft.AspNetCore.Mvc;
using Bug_Ticketing_System.BL.Mangers.Attachments;
using Bug_Ticketing_System.BL.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Bug_Ticketing_System.Controllers
{
    [Route("api/bugs")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentManager _attachmentManager;

        public AttachmentController(IAttachmentManager attachmentManager)
        {
            _attachmentManager = attachmentManager;
        }

        [HttpPost("{bugId}/attachments")]
        [Authorize]
        public async Task<IActionResult> UploadAttachment(Guid bugId, [FromForm] AttachmentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var attachment = await _attachmentManager.UploadAttachmentAsync(bugId, request, userId.Value);
                return CreatedAtAction(nameof(GetAttachments), new { bugId }, attachment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{bugId}/attachments")]
        public async Task<IActionResult> GetAttachments(Guid bugId)
        {
            var attachments = await _attachmentManager.GetAttachmentsAsync(bugId);
            return Ok(attachments);
        }

        [HttpDelete("{bugId}/attachments/{attachmentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAttachment(Guid bugId, Guid attachmentId)
        {
   
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();
            try
            {
                await _attachmentManager.DeleteAttachmentAsync(bugId, attachmentId, userId.Value);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out Guid userId))
                return userId;
            return null;
        }
    }
}
