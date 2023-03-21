using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using BugSpy.Services;
using BugSpy.Extensions;
using System.ComponentModel.Design;
using BugSpy.Models.Enums;
using BugSpy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugSpy.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _fileService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _btTicketService;
        private readonly IBTProjectService _btProjectService;
        private readonly IBTRolesService _roleService;
        private readonly IBTTicketHistoryService _historyService;
        private readonly IBTNotificationService _notificationService;
        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTTicketService btTicketService, IBTFileService fileService, IBTRolesService roleService, IBTTicketHistoryService historyService, IBTNotificationService notificationService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _btTicketService = btTicketService;
            _fileService = fileService;
            _roleService = roleService;
            _historyService = historyService;
            _notificationService = notificationService;
            _btProjectService = projectService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {

            IEnumerable<Ticket> tickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();

            return View(tickets);
        }








        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDev(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();

            IEnumerable<BTUser> developers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Developer), companyId);

            BTUser? currentDev = await _btTicketService.GetDeveloperAsync(Id);

            AssignDevViewModel viewModel = new()
            {

                Ticket = await _btTicketService.GetTicketByIdAsync(Id, companyId),
                DevList = new SelectList(developers, "Id", "FullName", currentDev?.Id),
                DevId = currentDev?.Id


            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDev(AssignDevViewModel viewModel)
        {
            int companyId = User.Identity.GetCompanyId();



            Ticket oldTicket = await _btTicketService.GetTicketAsNoTracking(viewModel.Ticket.Id, companyId);



            if (!string.IsNullOrEmpty(viewModel.DevId))
            {
                await _btTicketService.AssignDeveloperAsync(viewModel.DevId, viewModel.Ticket?.Id);

                Ticket newTicket = await _btTicketService.GetTicketAsNoTracking(viewModel.Ticket.Id, companyId);

                string? userId = _userManager.GetUserId(User);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);






				BTUser? btUser = await _userManager.GetUserAsync(User);


				Notification? notification = new()
				{
					TicketId = viewModel.Ticket.Id,

					Title = "Developer Assigned",

					Message = $"Ticket: {viewModel.Ticket.Title} was assigned by {btUser.FullName}",

					Created = DataUtility.GetPostGresDate(DateTime.Now),

					SenderId = userId,

					RecipientId = viewModel.DevId,

					NotificationTypeId = (await _context.NotificationsTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id,


				};


			
					await _notificationService.AddNotificationAsync(notification);
					await _notificationService.SendEmailNotficationAsync(notification, "Developer Assigned");

				






				return RedirectToAction("Details", new { id = viewModel.Ticket?.Id });
            }


            ModelState.AddModelError("DevID", "No Developer chosen. Please Select a developer");




            IEnumerable<BTUser> developers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Developer), companyId);

            BTUser? currentDev = await _btTicketService.GetDeveloperAsync(viewModel.Ticket.Id);

            viewModel.Ticket = await _btTicketService.GetTicketByIdAsync(viewModel.Ticket.Id, companyId);

            viewModel.DevList = new SelectList(developers, "Id", "FullName", currentDev?.Id);






            return View(viewModel);



        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllUnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId();




            IEnumerable<Ticket> unassignedCompanyTickets = await _btTicketService.GetAllCompanyUnassignedTickets(companyId);




            return View(unassignedCompanyTickets);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllAssignedTickets()
        {
            int companyId = User.Identity.GetCompanyId();





            IEnumerable<Ticket> assignedCompanyTickets = await _btTicketService.GetAllCompanyAssignedTickets(companyId);




            return View(assignedCompanyTickets);
        }


        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> UnassignedPMTickets()
        {


            string PMUserId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId();



       


            IEnumerable<Ticket> UnassignedPMTickets = await _btTicketService.GetUnassignedPMTickets( PMUserId, companyId);


        
            return View(UnassignedPMTickets);





        }
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> AssignedPMTickets()
        {


            string PMUserId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId();





            IEnumerable<Ticket> AssignedPMTickets = await _btTicketService.GetAssignedPMTickets(PMUserId, companyId);

            return View(AssignedPMTickets);






        }


        public async Task<IActionResult> TicketsInProject(int? Id)
        {

            IEnumerable<Ticket> projectTickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).Where(t => t.ProjectId == Id).ToListAsync();



            return View(projectTickets);
        }






        public async Task<IActionResult> TabulatorTickets()
        {

            int companyId = User.Identity!.GetCompanyId();

          

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {

                IEnumerable<Ticket> recentSubmitterTickets = (await _btTicketService.GetSubmitterRecentTickets(userId));
                return View(recentSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IEnumerable<Ticket> recentDeveloperTickets = (await _btTicketService.GetDeveloperRecentTickets(userId));
                return View(recentDeveloperTickets);
            }
            else if(User.IsInRole("Admin"))
            {
                IEnumerable<Ticket> recentSubmitterTickets = (await _btTicketService.GetAllCompanyTickets(companyId));

                return View(recentSubmitterTickets);
            } else
            {
                IEnumerable<Ticket> recentSubmitterTickets = (await _btTicketService.GetAllCompanyTickets(companyId));

                return View(recentSubmitterTickets);
            }




        }



        public async Task<IActionResult> TicketsIndex()
        {

            int companyId = User.Identity!.GetCompanyId();



            string? userId = _userManager.GetUserId(User);

           
                IEnumerable<Ticket> companyTickets = (await _btTicketService.GetAllCompanyActiveTickets(companyId));
                return View(companyTickets);
        




        }


        public async Task<IActionResult> CompanyArchivedTickets()
        {

            int companyId = User.Identity!.GetCompanyId();



            string? userId = _userManager.GetUserId(User);


            IEnumerable<Ticket> companyTickets = (await _btTicketService.GetAllCompanyArchivedTickets(companyId));
            return View(companyTickets);





        }






        public async Task<IActionResult> MyRecentTickets(int? pageNum)
        {

            int companyId = User.Identity!.GetCompanyId();

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter")) {

                IPagedList<Ticket> recentSubmitterTickets = (await _btTicketService.GetSubmitterRecentTickets(userId)).ToPagedList(page, pageSize);
                return View(recentSubmitterTickets);
            } else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> recentDeveloperTickets = (await _btTicketService.GetDeveloperRecentTickets(userId)).ToPagedList(page, pageSize);
                return View(recentDeveloperTickets);
            } else
            {
                IPagedList<Ticket> recentSubmitterTickets = (await _btTicketService.GetSubmitterRecentTickets(userId)).ToPagedList(page, pageSize);

                return View(recentSubmitterTickets);
            }




        }


        [HttpGet] 
        public async Task<IActionResult> GetCompanyTicketHistory()
        {

            int companyId = User.Identity.GetCompanyId();

            IEnumerable<TicketHistory> ticketHistories = await _historyService.GetCompanyTicketHistoriesAsync(companyId);

            return View(ticketHistories);

        }

        public async Task<IActionResult> MyTicketsType(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
			{
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> typeSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByType(userId)).ToPagedList(page, pageSize);
                return View(typeSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> typeDeveloperTickets = (await _btTicketService.GetDeveloperTicketsByType(userId)).ToPagedList(page, pageSize);
                return View(typeDeveloperTickets);
            }
            else
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> typeSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByType(userId)).ToPagedList(page, pageSize);

                return View(typeSubmitterTickets);
            }


        }


        public async Task<IActionResult> MyTicketsPriority(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> prioritySubmitterTickets = (await _btTicketService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(prioritySubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> priorityDeveloperTickets = (await _btTicketService.GetDeveloperTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(priorityDeveloperTickets);
            }
            else
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> prioritySubmitterTickets = (await _btTicketService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);

                return View(prioritySubmitterTickets);
            }


        }


        public async Task<IActionResult> MyTicketsStatus(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> statusSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(statusSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> statusDeveloperTickets = (await _btTicketService.GetDeveloperTicketsByStatus(userId)).ToPagedList(page, pageSize);
                return View(statusDeveloperTickets);
            }
            else
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> statusSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);

                return View(statusSubmitterTickets);
            }


        }


        public async Task<IActionResult> MyTicketsTitle(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> titleSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(titleSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> titleDeveloperTickets = (await _btTicketService.GetDeveloperTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(titleDeveloperTickets);
            }
            else
            {

				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> titleSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByTitle(userId)).ToPagedList(page, pageSize);

                return View(titleSubmitterTickets);
            }


        }


        public async Task<IActionResult> MyTicketsProject(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> projectSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByProject(userId)).ToPagedList(page, pageSize);
                return View(projectSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> projectDeveloperTickets = (await _btTicketService.GetDeveloperTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(projectDeveloperTickets);
            }
            else
            {
				int companyId = User.Identity!.GetCompanyId();
				IPagedList<Ticket> projectSubmitterTickets = (await _btTicketService.GetSubmitterTicketsByProject(userId)).ToPagedList(page, pageSize);

                return View(projectSubmitterTickets);
            }


        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
		{
			string statusMessage;

            ModelState.Remove("BTUserId");


			if (ModelState.IsValid && ticketAttachment.FormFile != null)
			{
				ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;
                ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                ticketAttachment.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
				ticketAttachment.BTUserId = _userManager.GetUserId(User);

				await _btTicketService.AddTicketAttachmentAsync(ticketAttachment);

                await _historyService.AddHistoryAsync(ticketAttachment.TicketId, nameof(TicketAttachment), ticketAttachment.BTUserId);
                statusMessage = "Success: New attachment added to Ticket.";
			}
			else
			{
				statusMessage = "Error: Invalid data.";

			}

			return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
		}






		public async Task<IActionResult> ShowFile(int id)
		{
			TicketAttachment ticketAttachment = await _btTicketService.GetTicketAttachmentByIdAsync(id);
			string fileName = ticketAttachment.FileName;
			byte[] fileData = ticketAttachment.FileData;
			string ext = Path.GetExtension(fileName).Replace(".", "");

			Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
			return File(fileData, $"Application/{ext}");
		}





		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddCommentToTicket([Bind("Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
		{

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
			{
				ticketComment.UserId = _userManager.GetUserId(User);

				ticketComment.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                int ticketId = ticketComment.TicketId;

				_context.Add(ticketComment);
				await _context.SaveChangesAsync();

                await _historyService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);

				return RedirectToAction("Details", "Tickets", new { id = ticketId });
			}

			return View(ticketComment);
		}


















		// GET: Tickets/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            int companyId = User.Identity.GetCompanyId();

            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _btTicketService.GetTicketByIdAsync(id, companyId);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()


        {

            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            int companyId = loggedInUser!.CompanyId;


        
            ViewData["ProjectId"] = new SelectList(await _btTicketService.GetProjectListAsync(companyId), "Id", "Name");
     

            ViewData["TicketPriorityId"] = new SelectList(await _btTicketService.GetTicketPriorities(), "Id", "Name");
  
            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");
     
            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            ModelState.Remove("SubmitterUserId");
            int companyId = User.Identity.GetCompanyId();
            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);
                ticket.SubmitterUserId = _userManager.GetUserId(User);

                BTUser? btUser = await _userManager.GetUserAsync(User);





                ticket.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);



                _context.Add(ticket);
                await _context.SaveChangesAsync();


                
                //int companyId = User.Identity.GetCompanyId();
                Ticket newTicket = await _btTicketService.GetTicketAsNoTracking(ticket.Id, companyId);

                await _historyService.AddHistoryAsync(null, newTicket, userId);

                BTUser? projectManager = await _btProjectService.GetProjectManagerAsync(ticket.ProjectId);

                Notification? notification = new()
                {
                    TicketId = ticket.Id,

                    Title = "New Ticket Added",

                    Message = $"New Ticket: {ticket.Title} was created by {btUser.FullName}",

                    Created = DataUtility.GetPostGresDate(DateTime.Now),

                    SenderId = userId,

                    RecipientId = projectManager?.Id,

                    ProjectId = ticket.ProjectId,   

                    NotificationTypeId = (await _context.NotificationsTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id,


                };


                if(projectManager != null)
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotficationAsync(notification, "New Ticket Added");

                } else
                {

                    await _notificationService.AdminNotficationAsync(notification, companyId);
                    await _notificationService.SendAdminEmailNotficiationAsync(notification, "New Ticket Added", companyId);

                }
                return RedirectToAction(nameof(TabulatorTickets));
            }



            ViewData["ProjectId"] = new SelectList(await _btTicketService.GetProjectListAsync(companyId), "Id", "Name");


            ViewData["TicketPriorityId"] = new SelectList(await _btTicketService.GetTicketPriorities(), "Id", "Name");

            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");

            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            int companyId = User.Identity.GetCompanyId();

            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User);

            Ticket? ticket = await _btTicketService.GetTicketByIdAsync(id, companyId);

            if (ticket == null)
            {
                return NotFound();
            }
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(await _btTicketService.GetProjectListAsync(companyId), "Id", "Name");


            ViewData["TicketPriorityId"] = new SelectList(await _btTicketService.GetTicketPriorities(), "Id", "Name");

            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");

            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            int companyId = User.Identity.GetCompanyId();
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                string userId = _userManager.GetUserId(User);

             

                Ticket oldTicket = await _btTicketService.GetTicketAsNoTracking(ticket.Id, companyId);


                try
                {

          


                    ticket.Created = DataUtility.GetPostGresDate(ticket.Created);

                    ticket.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);

                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Ticket newTicket = await _btTicketService.GetTicketAsNoTracking(ticket.Id, companyId);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);


				BTUser? btUser = await _userManager.GetUserAsync(User);

				BTUser? projectManager = await _btProjectService.GetProjectManagerAsync(ticket.ProjectId);

				Notification? notification = new()
				{
					TicketId = ticket.Id,

					Title = "New Ticket Added",

					Message = $"New Ticket: {ticket.Title} was created by {btUser.FullName}",

					Created = DataUtility.GetPostGresDate(DateTime.Now),

					SenderId = userId,

					RecipientId = projectManager?.Id,

					NotificationTypeId = (await _context.NotificationsTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id,

                    ProjectId = ticket.ProjectId


				};


				if (projectManager != null)
				{
					await _notificationService.AddNotificationAsync(notification);
					await _notificationService.SendEmailNotficationAsync(notification, "New Ticket Added");

				}
				else
				{

					await _notificationService.AdminNotficationAsync(notification, companyId);
					await _notificationService.SendAdminEmailNotficiationAsync(notification, "New Ticket Added", companyId);

				}




				return RedirectToAction(nameof(TabulatorTickets));
            }

            ViewData["ProjectId"] = new SelectList(await _btTicketService.GetProjectListAsync(companyId), "Id", "Name");


            ViewData["TicketPriorityId"] = new SelectList(await _btTicketService.GetTicketPriorities(), "Id", "Name");

            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");

            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int companyId = User.Identity.GetCompanyId();

            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _btTicketService.GetTicketByIdAsync(id, companyId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            int companyId = User.Identity.GetCompanyId();

            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            Ticket? ticket = await _btTicketService.GetTicketByIdAsync(id, companyId);
            if (ticket != null)
            {
                ticket.Archived = true;

                if(ticket.Project.Archived == true)
                {

                    ticket.Archived = true;    
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
