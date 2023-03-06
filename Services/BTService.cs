﻿using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugSpy.Services
{
    public class BTService : IBTService
    {

		private readonly ApplicationDbContext _context;
		private readonly UserManager<BTUser> _userManager;

		public BTService(ApplicationDbContext context, UserManager<BTUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


        public async Task MembersToProject(IList<string> userIds, int projectId)
        {

			try
			{


				Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

				foreach(string userId in userIds)
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
    }
}