using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
    public interface IBTProjectService
    {
        public Task<Project> GetProjectByIdAsync(int companyId, int? projectId);

		public Task MembersToProject(IList<string> userIds, int projectId);


		public Task RemoveAllMembersFromProject(int projectId);


		public Task<IEnumerable<Project>> GetSignedInUserProjects(int companyId, string? userId);

	}
}
