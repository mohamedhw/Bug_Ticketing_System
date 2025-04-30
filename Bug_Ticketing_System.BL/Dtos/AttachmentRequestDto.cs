using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bug_Ticketing_System.BL.Dtos
{
    public class AttachmentRequestDto
    {
        [Required(ErrorMessage = "File is required.")]
        public required IFormFile File { get; set; }
    }
}
