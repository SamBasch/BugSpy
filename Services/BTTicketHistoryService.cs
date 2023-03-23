using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BugSpy.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _roleService;

        public BTTicketHistoryService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService roleService)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
        }

        public async Task AddHistoryAsync(int? ticketId, string? model, string? userId)
        {
            try
            {
                  Ticket ticket = await _context.Tickets.FindAsync(ticketId);
                  string description = model.ToLower().Replace("ticket", "");
                  description = $"New {description} added to ticket: {ticket.Title}";

                if(ticket != null)
                {

                    TicketHistory history = new()
                    {
                        TicketId = ticket.Id,
                        PropertyName = model,
                        OldValue = string.Empty,
                        NewValue = string.Empty,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = description

                    };

                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
 
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddHistoryAsync(Ticket? oldTicket, Ticket? newTicket, string? userId)
        {
            if(oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    PropertyName = string.Empty,
                    OldValue = string.Empty,
                    NewValue = string.Empty, 
                    Created = DataUtility.GetPostGresDate(DateTime.UtcNow), 
                    UserId = userId, 
                    Description = "New Ticket Created"

                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }


            } else if (oldTicket != null && newTicket != null)
            {
                //check title
                if(oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Title from: {oldTicket.Title} to : {newTicket.Title}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }

                //check Description
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Description from: {oldTicket.Description} to : {newTicket.Description}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }

                //check Priority
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Priority",
                        OldValue = oldTicket.TicketPriority?.Name,
                        NewValue = newTicket.TicketPriority?.Name,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Priority from: {oldTicket.TicketPriority?.Name} to : {newTicket.TicketPriority?.Name}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }

                //check Status
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Status",
                        OldValue = oldTicket.TicketStatus?.Name,
                        NewValue = newTicket.TicketStatus?.Name,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Status from: {oldTicket.TicketStatus?.Name} to : {newTicket.TicketStatus?.Name}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }


                //check Type
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Type",
                        OldValue = oldTicket.TicketType?.Name,
                        NewValue = newTicket.TicketType?.Name,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Type from: {oldTicket.TicketType?.Name} to : {newTicket.TicketType?.Name}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }


                //check Ticket Developer
                if (oldTicket.DeveloperUser != newTicket.DeveloperUser)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DataUtility.GetPostGresDate(DateTime.UtcNow),
                        UserId = userId,
                        Description = $"Changed Ticket Developer from: {oldTicket.DeveloperUser?.FullName} to : {newTicket.DeveloperUser?.FullName}"

                    };

                    await _context.TicketHistories.AddAsync(history);


                }


                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int? companyId)
        {


            List<TicketHistory> ticketHistories = (await _context.TicketHistories.Include(t => t.User).Include(th => th.Ticket).ThenInclude(t => t.Project).Where(t => t.Ticket.Project.CompanyId == companyId).ToListAsync());

            //Company? company = await _context.Companies.Include(c => c.Projects).ThenInclude(p => p.Tickets).ThenInclude(t => t.History).FirstOrDefaultAsync(c => c.Id == companyId);


            //IEnumerable<Ticket> companyTickets = company.Projects.SelectMany(p => p.Tickets.Where(t => t.Archived == false));


            //List<TicketHistory> ticketHistories = companyTickets.SelectMany(t => t.History).ToList();



            return ticketHistories;
        }



        public async Task<List<TicketHistory>> GetSingularTicketsHistory(int? companyId, int? ticketId)
        {


            List<TicketHistory> ticketHistories = (await _context.TicketHistories.Include(t => t.User).Include(th => th.Ticket).ThenInclude(t => t.Project).Where(t => t.Ticket.Project.CompanyId == companyId && t.TicketId == ticketId).ToListAsync());

            //Company? company = await _context.Companies.Include(c => c.Projects).ThenInclude(p => p.Tickets).ThenInclude(t => t.History).FirstOrDefaultAsync(c => c.Id == companyId);


            //IEnumerable<Ticket> companyTickets = company.Projects.SelectMany(p => p.Tickets.Where(t => t.Archived == false));


            //List<TicketHistory> ticketHistories = companyTickets.SelectMany(t => t.History).ToList();



            return ticketHistories;
        }




        public async Task<List<TicketHistory>> GetProjectTicketsHistory(int? companyId, int? projectId)
        {


            List<TicketHistory> ticketHistories = (await _context.TicketHistories.Include(t => t.User).Include(th => th.Ticket).ThenInclude(t => t.Project).Where(t => t.Ticket.Project.CompanyId == companyId && t.Ticket.ProjectId == projectId).ToListAsync());

            //Company? company = await _context.Companies.Include(c => c.Projects).ThenInclude(p => p.Tickets).ThenInclude(t => t.History).FirstOrDefaultAsync(c => c.Id == companyId);


            //IEnumerable<Ticket> companyTickets = company.Projects.SelectMany(p => p.Tickets.Where(t => t.Archived == false));


            //List<TicketHistory> ticketHistories = companyTickets.SelectMany(t => t.History).ToList();



            return ticketHistories;
        }


        public Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int? projectId, int? companyId)
        {
            throw new NotImplementedException();
        }
    }
}
