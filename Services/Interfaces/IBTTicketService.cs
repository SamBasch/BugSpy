using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
	public interface IBTTicketService
	{

		public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);

		public Task<IEnumerable<Ticket>> GetAllCompanyTickets(int? companyId);

		public Task<IEnumerable<Ticket>> GetAllCompanyActiveTickets(int? companyId);

		public Task<IEnumerable<Ticket>> GetAllCompanyArchivedTickets(int? companyId);


        public Task<Ticket> GetTicketAsNoTracking(int? ticketId, int companyId);

        public Task<IEnumerable<Ticket>> GetSubmitterRecentTickets(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByType(string? userId);

		public Task<BTUser> GetDeveloperAsync(int? ticketId);

		public Task AssignDeveloperAsync(string userId, int? ticketId);
        public Task<Ticket> GetTicketByIdAsync(int? ticketId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByPriority(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeEnhancement(string? userId);

		public Task<IEnumerable<Ticket>> GetUnassignedPMTickets(string? userId, int? companyId);


		public Task<IEnumerable<Ticket>> GetAssignedPMTickets(string? userId, int? companyId);

        public Task<IEnumerable<BTUser>> GetUsersTeamMembers(BTUser btUser, int companyId);

		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeNewDevelopment(string? userId, int? companyId);

		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeWorkTask(string? userId, int? companyId);

		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeDefect(string? userId, int? companyId);

		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeChangeRequest(string? userId, int? companyId);

		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeEnhancement(string? userId, int? companyId);


		public Task<IEnumerable<Ticket>> GetPMTicketsWithTypeGeneralTask(string? userId, int? companyId);


        public Task<IEnumerable<Ticket>> GetPMTicketsHighPriority(string? userId, int? companyId);
        public Task<IEnumerable<Ticket>> GetSubmitterTicketsUrgentAndHighPriority(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsUrgentAndHighPriority(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeNewDevelopment(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeWorkTask(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeGeneralTask(string? userId);


        public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeDefect(string? userId);

		public Task<IEnumerable<Ticket>> GetDevTicketsWithTypeChangeRequest(string? userId);

        public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeChangeRequest(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeNewDevelopment(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeWorkTask(string? userId);
		public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeDefect(string? userId);

		public Task<IEnumerable<Ticket>> GetRecentCompanyTickets();


        public Task<IEnumerable<Ticket>> GetSubmitterTicketsByStatus(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByTitle(string? userId);


		public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeEnhancement(string? userId);



        public Task<IEnumerable<Ticket>> GetSubmitterTicketsByProject(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsWithTypeGeneralTask(string? userId);



        public Task<IEnumerable<Ticket>> GetDeveloperRecentTickets(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByType(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByPriority(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByStatus(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByTitle(string? userId);


		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByProject(string? userId);

		public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
	}
}
