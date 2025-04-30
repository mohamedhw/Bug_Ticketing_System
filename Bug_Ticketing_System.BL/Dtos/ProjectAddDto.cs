

namespace Bug_Ticketing_System.BL.Dtos
{
    public class ProjectAddDto
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ManagerId { get; set; }
        public List<Guid> TicketIds { get; set; } = new List<Guid>();
    }
}
