public class ProjectDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; }
    public List<Guid> BugIds { get; set; } = new();
}