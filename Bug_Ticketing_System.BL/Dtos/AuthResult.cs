
namespace Bug_Ticketing_System.BL.Dtos
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
