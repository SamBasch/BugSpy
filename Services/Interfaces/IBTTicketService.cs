using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
	public interface IBTTicketService
	{

		public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);



		public Task<IEnumerable<Ticket>> GetSubmitterRecentTickets(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByType(string? userId);



		public Task<Ticket> GetTicketByIdAsync(int? ticketId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByPriority(string? userId);


		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByStatus(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByTitle(string? userId);

		public Task<IEnumerable<Ticket>> GetSubmitterTicketsByProject(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperRecentTickets(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByType(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByPriority(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByStatus(string? userId);

		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByTitle(string? userId);


		public Task<IEnumerable<Ticket>> GetDeveloperTicketsByProject(string? userId);

		public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
	}
}
