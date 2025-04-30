
using Bug_Ticketing_System.DAL.Repos;


namespace Bug_Ticketing_System.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IProjectRepository ProjectRepository { get; }
        public IUserRepository  UserRepository { get; }
        public IAttachmentRepository AttachmentRepository { get; }
        public IBugAssigneeRepository BugAssigneeRepository { get; }
        public IBugRepository BugRepository { get; }

        Task<int> SaveChangesAsync();

    }
}
