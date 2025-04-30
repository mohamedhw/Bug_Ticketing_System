using Bug_Ticketing_System.DAL.Repos;


namespace Bug_Ticketing_System.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BugTicketingSystemContext _context;

        public IProjectRepository ProjectRepository { get; }
        public IUserRepository UserRepository { get; }
        public IAttachmentRepository AttachmentRepository { get; }
        public IBugAssigneeRepository BugAssigneeRepository { get; }
        public IBugRepository BugRepository { get; }


        public UnitOfWork(
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IAttachmentRepository attachmentRepository,
            IBugAssigneeRepository bugAssigneeRepository,
            IBugRepository bugRepository,
            BugTicketingSystemContext context
            )
        {
            ProjectRepository = projectRepository;
            UserRepository = userRepository;
            AttachmentRepository = attachmentRepository;
            BugAssigneeRepository = bugAssigneeRepository;
            BugRepository = bugRepository;
            _context = context;
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
