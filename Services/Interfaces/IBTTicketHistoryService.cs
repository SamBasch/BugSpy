using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
    public interface IBTTicketHistoryService
    {


        Task AddHistoryAsync(Ticket? oldTicket, Ticket? newTicket, string? userId);


        Task AddHistoryAsync(int? ticketId, string? model, string? userId);

        Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int? projectId, int? companyId);

        public Task<List<TicketHistory>> GetProjectTicketsHistory(int? companyId, int? projectId);

        public Task<List<TicketHistory>> GetSingularTicketsHistory(int? companyId, int? ticketId);

        Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int? companyId);
    }
}
