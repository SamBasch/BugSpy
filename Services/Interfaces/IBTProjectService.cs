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

		public Task<bool> RemoveMemberFromProjectAsync(BTUser member, int? projectId);

		public Task<bool> AddMemberToProjectAsync(BTUser member, int? projectId);

		public Task<bool> IsMemberInProject(BTUser member, int? projectId);


    }
}
