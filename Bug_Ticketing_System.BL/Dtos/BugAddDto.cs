

namespace Bug_Ticketing_System.BL.Dtos
{
    public class BugAddDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public Guid ProjectId { get; set; }

    }
}
