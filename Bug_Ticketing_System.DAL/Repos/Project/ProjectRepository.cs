using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bug_Ticketing_System.DAL
{
    public class ProjectRepository:IProjectRepository
    {
        private readonly BugTicketingSystemContext _context;

        public ProjectRepository(BugTicketingSystemContext context)
        {
            _context = context;
        }

        public async Task AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public async Task<Project> GetProjectDetailAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Bugs)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

        }
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Bugs)
                .ToListAsync();
        }
    }
}
