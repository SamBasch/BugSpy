using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugSpy.Services
{
    public class BTProjectService : IBTProjectService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;


        public BTProjectService(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<Project> GetProjectByIdAsync(int companyId, int? projectId)
        {
            try
			{
                Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).ThenInclude(t => t.TicketPriority).Include(p => p.Tickets).ThenInclude(t => t.TicketType).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == projectId);

                return project;
            }
            catch (Exception)
			{

				throw;
			}

        }


		public async Task MembersToProject(IList<string> userIds, int projectId)
		{

			try
			{


				Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

				foreach (string userId in userIds)
				{

					BTUser? bTUser = await _context.Users.FindAsync(userId);

					project.Members.Add(bTUser);
				}

				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{

				throw;
			}


		}


		public async Task RemoveAllMembersFromProject(int projectId)
		{


			try
			{

				Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

				project!.Members.Clear();

				_context.Update(project);

				await _context.SaveChangesAsync();

			}
			catch (Exception)
			{

				throw;
			}





		}



		public async Task<IEnumerable<Project>> GetSignedInUserProjects(int companyId, string? userId)
		{


			try
			{
				IEnumerable<Project> projects = await _context.Projects.Where(p => p.Members.Any(m => m.Id == userId) && p.CompanyId == companyId).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();

				return projects.OrderByDescending(p => p.Created);
			}
			catch (Exception)
			{

				throw;
			}



		}



	}
}
