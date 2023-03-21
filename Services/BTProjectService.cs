using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Models.Enums;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugSpy.Services
{
    public class BTProjectService : IBTProjectService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
		private readonly IBTRolesService _roleService;


        public BTProjectService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService roleService)
        {
            _context = context;
            _userManager = userManager;
			_roleService = roleService;

        }
        public async Task<Project> GetProjectByIdAsync(int? companyId, int? projectId)
        {
            try
			{
                Project? project = await _context.Projects.Where(p => p.CompanyId == companyId).Include(p => p.Members).Include(p => p.Tickets).ThenInclude(t => t.TicketPriority).Include(p => p.Tickets).ThenInclude(t => t.TicketType).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == projectId);

                return project;
            }
            catch (Exception)
			{

				throw;
			}

        }


		public async Task AddMembersToProject(IList<string> userIds, int? projectId, int? companyId)
		{

			try
			{


				Project? project = await GetProjectByIdAsync(companyId, projectId);

				foreach (string userId in userIds)
				{

					BTUser? btUser = await _context.Users.FindAsync(userId);


					if(btUser != null && project != null)
					{

						bool IsOnProject = project.Members.Any(m => m.Id == userId);

						if(!IsOnProject)
						{

							project.Members.Add(btUser);
						} else
						{

							continue;
						}

                        

                    }


                  
                }

				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{

				throw;
			}


		}


		public async Task RemoveAllMembersFromProject(int? projectId, int? companyId)
		{


			try
			{

				Project? project = await GetProjectByIdAsync(companyId, projectId);


				foreach(BTUser member in project.Members)
				{

					if(!await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
					{

						project.Members.Remove(member);
					}



				}

				//project!.Members.Clear();

				//_context.Update(project);

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

		public async Task<bool> AddProjectManagerAsync(string? userId, int? projectId)
		{
			try
			{

				BTUser? currentPM = await GetProjectManagerAsync(projectId);
				BTUser? selectedPM = await _context.Users.FindAsync(userId);


				if(currentPM != null)
				{
					await RemoveProjectManagerAsync(projectId);
				}

				try
				{
					await AddMemberToProjectAsync(selectedPM!, projectId);
					return true;
				}
				catch (Exception)
				{

					throw;
				}
				


			}
			catch (Exception)
			{

				throw;
			}
		}



        public async Task<IEnumerable<Project>> GetAllCompanyProjects(int companyId)
        {





            try
            {
                IEnumerable<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();

                return projects.OrderByDescending(p => p.Created);
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<IEnumerable<Project>> GetAllCompanyActiveProjects(int companyId)
        {





            try
            {
                IEnumerable<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();

                return projects.OrderByDescending(p => p.Created);
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<IEnumerable<Project>> GetAllCompanyArchivedProjects(int companyId)
        {





            try
            {
                IEnumerable<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();

                return projects.OrderByDescending(p => p.Created);
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<IEnumerable<ProjectPriority>> GetProjectPriorityList()
		{
			IEnumerable<ProjectPriority> priorityList = await _context.ProjectPriorities.ToListAsync();

			return priorityList;	
		}

        public async Task<bool> IsMemberInProject(BTUser member, int? projectId)
		{
            Project project = await GetProjectByIdAsync(member!.CompanyId, projectId);

            bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

            if (!IsOnProject)
            {
             
                return true;

            }

            return false;

        }

		public async Task ArchiveProjectTickets(Project project)
		{
			


			


            if (project != null && project.Archived == true)
            {
                foreach (Ticket ticket in project.Tickets) 
				
				{
					ticket.ArchivedByProject = true;
					ticket.Archived = true;
                    _context.Update(ticket);
               
                }
		

			}
            await _context.SaveChangesAsync();

        }




        public async Task UnarchiveProjectTickets(Project project)
        {






            if (project != null && project.Archived == false)
            {
				

                foreach (Ticket ticket in project.Tickets)

                {

					if(ticket.ArchivedByProject == true)
					{
                        ticket.ArchivedByProject = false;
                        ticket.Archived = false;
                    }

          
                    _context.Update(ticket);

                }


            }
            await _context.SaveChangesAsync();

        }


        public async Task<BTUser> GetProjectManagerAsync(int? projectId)
		{
			try
			{
				Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

				foreach(BTUser member in project.Members)
				{
					if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
					{
						return member;
					}

				}

				return null!;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task RemoveProjectManagerAsync(int? projectId)
		{
			try
			{
				Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

				foreach (BTUser member in project.Members)
				{
					if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
					{
						await RemoveMemberFromProjectAsync(member, projectId);
					}

				}

				return;
            }

			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> RemoveMemberFromProjectAsync(BTUser member, int? projectId)
		{
			try
			{
				Project? project = await GetProjectByIdAsync(member.CompanyId, projectId);

				bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

				if (IsOnProject)
				{
					project.Members.Remove(member);
					await _context.SaveChangesAsync();
					return true;
				}

				return false;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> AddMemberToProjectAsync(BTUser member, int? projectId)
		{
			try
			{
				Project project = await GetProjectByIdAsync(member!.CompanyId, projectId );

				bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

				if(!IsOnProject)
				{
					project.Members.Add(member);
					await _context.SaveChangesAsync();
					return true;

				}

				return false;
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
