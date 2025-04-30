

namespace Bug_Ticketing_System.BL.Dtos
{
    public class UserAddDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
