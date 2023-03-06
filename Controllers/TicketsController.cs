using System;
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

namespace BugSpy.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _fileServce;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTService _btService;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTService btService)
        {
            _context = context;
            _userManager = userManager;
            _btService = btService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {

            IEnumerable<Ticket> tickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).ToListAsync();
           
            return View(tickets);
        }



        public async Task<IActionResult> TicketsInProject(int? Id)
        {

            IEnumerable<Ticket> projectTickets = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).Where(t => t.ProjectId == Id).ToListAsync();



            return View(projectTickets);
        }

        public async Task<IActionResult> MyRecentTickets(int? pageNum)
        {



            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if(User.IsInRole("Submitter")) {

                IPagedList<Ticket> recentSubmitterTickets = (await _btService.GetSubmitterRecentTickets(userId)).ToPagedList(page, pageSize);
                return View(recentSubmitterTickets);
            } else if(User.IsInRole("Developer"))
            {
                IPagedList<Ticket> recentDeveloperTickets = (await _btService.GetDeveloperRecentTickets(userId)).ToPagedList(page, pageSize);
                return View(recentDeveloperTickets);
            } else
            {
                IPagedList<Ticket> recentSubmitterTickets = (await _btService.GetSubmitterRecentTickets(userId)).ToPagedList(page, pageSize);

                return View(recentSubmitterTickets);
            }




        }


        public async Task<IActionResult> MyTicketsType(int? pageNum)
        {

            int pageSize = 10;
            int page = pageNum ?? 1;

            string? userId = _userManager.GetUserId(User);

            if (User.IsInRole("Submitter"))
            {

                IPagedList<Ticket> typeSubmitterTickets = (await _btService.GetSubmitterTicketsByType(userId)).ToPagedList(page, pageSize);
                return View(typeSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> typeDeveloperTickets = (await _btService.GetDeveloperTicketsByType(userId)).ToPagedList(page, pageSize);
                return View(typeDeveloperTickets);
            }
            else
            {
                IPagedList<Ticket> typeSubmitterTickets = (await _btService.GetSubmitterTicketsByType(userId)).ToPagedList(page, pageSize);

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

                IPagedList<Ticket> prioritySubmitterTickets = (await _btService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(prioritySubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> priorityDeveloperTickets = (await _btService.GetDeveloperTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(priorityDeveloperTickets);
            }
            else
            {
                IPagedList<Ticket> prioritySubmitterTickets = (await _btService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);

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

                IPagedList<Ticket> statusSubmitterTickets = (await _btService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);
                return View(statusSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> statusDeveloperTickets = (await _btService.GetDeveloperTicketsByStatus(userId)).ToPagedList(page, pageSize);
                return View(statusDeveloperTickets);
            }
            else
            {
                IPagedList<Ticket> statusSubmitterTickets = (await _btService.GetSubmitterTicketsByPriority(userId)).ToPagedList(page, pageSize);

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

                IPagedList<Ticket> titleSubmitterTickets = (await _btService.GetSubmitterTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(titleSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> titleDeveloperTickets = (await _btService.GetDeveloperTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(titleDeveloperTickets);
            }
            else
            {
                IPagedList<Ticket> titleSubmitterTickets = (await _btService.GetSubmitterTicketsByTitle(userId)).ToPagedList(page, pageSize);

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

                IPagedList<Ticket> projectSubmitterTickets = (await _btService.GetSubmitterTicketsByProject(userId)).ToPagedList(page, pageSize);
                return View(projectSubmitterTickets);
            }
            else if (User.IsInRole("Developer"))
            {
                IPagedList<Ticket> projectDeveloperTickets = (await _btService.GetDeveloperTicketsByTitle(userId)).ToPagedList(page, pageSize);
                return View(projectDeveloperTickets);
            }
            else
            {
                IPagedList<Ticket> projectSubmitterTickets = (await _btService.GetSubmitterTicketsByProject(userId)).ToPagedList(page, pageSize);

                return View(projectSubmitterTickets);
            }


        }





        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket?  ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).FirstOrDefaultAsync(t => t.Id == id.Value);
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


            ViewData["DeveloperUserId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["SubmitterUserId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
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

            if (ModelState.IsValid)
            {

                ticket.SubmitterUserId = _userManager.GetUserId(User);

                ticket.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).FirstOrDefaultAsync(t => t.Id == id.Value);

            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(MyRecentTickets));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).FirstOrDefaultAsync(t => t.Id == id.Value);

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
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Comments).Include(t => t.Attachments).Include(t => t.History).FirstOrDefaultAsync(t => t.Id == id.Value);
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
