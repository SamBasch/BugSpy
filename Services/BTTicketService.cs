using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Models.Enums;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugSpy.Services
{
	public class BTTicketService : IBTTicketService
	{



		private readonly ApplicationDbContext _context;
		private readonly UserManager<BTUser> _userManager;

		public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}




		public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
		{
			try
			{
				await _context.AddAsync(ticketAttachment);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{

				throw;
			}
		}


		public async Task<Ticket> GetTicketByIdAsync(int? ticketId)
		{


			try
			{
				Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).FirstOrDefaultAsync(t => t.Id == ticketId);


				return ticket!;
			}
			catch (Exception)
			{

				throw;
			}
		}


		public async Task<IEnumerable<Ticket>> GetSubmitterRecentTickets(string? userId)
		{

			try
			{



				IEnumerable<Ticket> recentSubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return recentSubmitterTickets.OrderByDescending(rst => rst.Created);





			}
			catch (Exception)
			{

				throw;
			}




		}


		public async Task<IEnumerable<Ticket>> GetSubmitterTicketsByType(string? userId)
		{
			try
			{
				IEnumerable<Ticket> typeSubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return typeSubmitterTickets.OrderByDescending(rst => rst.TicketTypeId);
			}
			catch (Exception)
			{

				throw;
			}


		}


		public async Task<IEnumerable<Ticket>> GetSubmitterTicketsByPriority(string? userId)
		{
			try
			{
				IEnumerable<Ticket> prioritySubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return prioritySubmitterTickets.OrderByDescending(rst => rst.TicketPriorityId);
			}
			catch (Exception)
			{

				throw;
			}


		}



		public async Task<Ticket> GetTicketAsNoTracking(int? ticketId, int companyId)
		{

            Ticket newTicket = await _context.Tickets.Include(t => t.Project).ThenInclude(p => p!.Company).Include(t => t.Attachments).Include(t => t.Comments).Include(t => t.DeveloperUser).Include(t => t.History).
         
				
				Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticketId && t.Project.CompanyId == companyId && t.Archived == false);

			return newTicket;
        }

		public async Task<IEnumerable<Ticket>> GetSubmitterTicketsByStatus(string? userId)
		{
			try
			{
				IEnumerable<Ticket> statusSubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return statusSubmitterTickets.OrderByDescending(rst => rst.TicketStatusId);
			}
			catch (Exception)
			{

				throw;
			}


		}



		public async Task<IEnumerable<Ticket>> GetSubmitterTicketsByTitle(string? userId)
		{
			try
			{
				IEnumerable<Ticket> titleSubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return titleSubmitterTickets.OrderBy(rst => rst.Title);
			}
			catch (Exception)
			{

				throw;
			}


		}



		public async Task<IEnumerable<Ticket>> GetSubmitterTicketsByProject(string? userId)
		{
			try
			{
				IEnumerable<Ticket> projectSubmitterTickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return projectSubmitterTickets.OrderByDescending(rst => rst.ProjectId);
			}
			catch (Exception)
			{

				throw;
			}


		}




		public async Task<IEnumerable<Ticket>> GetDeveloperRecentTickets(string? userId)
		{

			try
			{



				IEnumerable<Ticket> recentDeveloperTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return recentDeveloperTickets.OrderByDescending(rdt => rdt.Created);





			}
			catch (Exception)
			{

				throw;
			}




		}

		public async Task<IEnumerable<Ticket>> GetDeveloperTicketsByType(string? userId)
		{

			try
			{



				IEnumerable<Ticket> typeDeveloperTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return typeDeveloperTickets.OrderByDescending(rdt => rdt.TicketTypeId);





			}
			catch (Exception)
			{

				throw;
			}




		}

		public async Task<IEnumerable<Ticket>> GetDeveloperTicketsByPriority(string? userId)
		{

			try
			{



				IEnumerable<Ticket> priorityDeveloperTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return priorityDeveloperTickets.OrderByDescending(rdt => rdt.TicketPriorityId);





			}
			catch (Exception)
			{

				throw;
			}




		}


		public async Task<IEnumerable<Ticket>> GetDeveloperTicketsByStatus(string? userId)
		{

			try
			{



				IEnumerable<Ticket> statusDeveloperTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return statusDeveloperTickets.OrderByDescending(rdt => rdt.TicketStatusId);





			}
			catch (Exception)
			{

				throw;
			}




		}


		public async Task<IEnumerable<Ticket>> GetDeveloperTicketsByTitle(string? userId)
		{

			try
			{



				IEnumerable<Ticket> titleDeveloperTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return titleDeveloperTickets.OrderBy(rdt => rdt.Title);





			}
			catch (Exception)
			{

				throw;
			}




		}

		public async Task<IEnumerable<Ticket>> GetDeveloperTicketsByProject(string? userId)
		{

			try
			{



				IEnumerable<Ticket> projectSubmitterTickets = await _context.Tickets.Where(t => t.DeveloperUserId == userId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
				return projectSubmitterTickets.OrderByDescending(rdt => rdt.ProjectId);





			}
			catch (Exception)
			{

				throw;
			}




		}


		public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
		{
			try
			{
				TicketAttachment ticketAttachment = await _context.TicketAttachments
																  .Include(t => t.BTUser)
																  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
				return ticketAttachment;
			}
			catch (Exception)
			{

				throw;
			}
		}




        //public async Task<bool> RemoveMemberFromTicketAsync(BTUser member, int? ticketId)
        //{
        //    try
        //    {
        //        Ticket? ticket = await GetTicketByIdAsync(ticketId);

        //        bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

        //        if (IsOnProject)
        //        {
        //            project.Members.Remove(member);
        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


    //    public async Task RemoveDeveloperAsync(int? ticketId)
    //    {
    //        try
    //        {
    //            Ticket ticket = await _context.Tickets.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);


				//if(ticket.DeveloperUser != null) {
				
				
				
				
				//}

				//ticket.Remove

    //            foreach (BTUser member in project.Members)
    //            {
    //                if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
    //                {
    //                    await RemoveMemberFromProjectAsync(member, projectId);
    //                }

    //            }

    //            return;
    //        }

    //        catch (Exception)
    //        {

    //            throw;
    //        }
    //    }


        public async Task<BTUser> GetDeveloperAsync(int? ticketId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);

				BTUser? developer = ticket.DeveloperUser;


				if(developer != null)
				{

					return developer;
				}

                //foreach (BTUser member in project.Members)
                //{
                //    if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                //    {
                //        return member;
                //    }

                //}

                return null!;
            }
            catch (Exception)
            {

                throw;
			}
		}



		public async Task AssignDeveloperAsync(string? userId, int? ticketId)
		{

			if(userId != null)
			{
                Ticket ticket = await _context.Tickets.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);

                BTUser? selectedDev = await _context.Users.FindAsync(userId);

				ticket.DeveloperUser = selectedDev;

                _context.Update(ticket);
                await _context.SaveChangesAsync();



            }
			return;
		}



		//public async Task<bool> AddDeveloperAsync(string? userId, int? ticketId)
		//{
		//    try
		//    {

		//        BTUser? currentDev = await GetDeveloperAsync(ticketId);
		//        BTUser? selectedDev = await _context.Users.FindAsync(userId);


		//        if (currentDev != null)
		//        {
		//            await RemoveProjectManagerAsync(projectId);
		//        }

		//        try
		//        {
		//            await AddMemberToProjectAsync(selectedPM!, projectId);
		//            return true;
		//        }
		//        catch (Exception)
		//        {

		//            throw;
		//        }



		//    }
		//    catch (Exception)
		//    {

		//        throw;
		//    }
		//}

	}
}
