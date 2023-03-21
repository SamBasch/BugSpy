using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
    public interface IBTProjectService
    {
        public Task<Project> GetProjectByIdAsync(int? companyId, int? projectId);

		public Task AddMembersToProject(IList<string> userIds, int? projectId, int? companyId);


		public Task RemoveAllMembersFromProject(int? projectId, int? companyId);


		public Task<IEnumerable<Project>> GetSignedInUserProjects(int companyId, string? userId);

		public Task<bool> AddProjectManagerAsync(string? userId, int? projectId);

		public Task<BTUser> GetProjectManagerAsync(int? projectId);

		public Task RemoveProjectManagerAsync(int? projectId);
		public Task<IEnumerable<ProjectPriority>> GetProjectPriorityList();

        public Task<IEnumerable<Project>> GetAllCompanyProjects(int companyId);

		public Task ArchiveProjectTickets(Project project);

        public Task<bool> RemoveMemberFromProjectAsync(BTUser member, int? projectId);

		public Task<bool> AddMemberToProjectAsync(BTUser member, int? projectId);

		public Task<bool> IsMemberInProject(BTUser member, int? projectId);


		public Task UnarchiveProjectTickets(Project project);




        public Task<IEnumerable<Project>> GetAllCompanyActiveProjects(int companyId);

		public Task<IEnumerable<Project>> GetAllCompanyArchivedProjects(int companyId);



    }
}
