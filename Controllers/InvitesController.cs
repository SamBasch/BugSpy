using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Extensions;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using BugSpy.Services;
using Microsoft.AspNetCore.Authorization;

namespace BugSpy.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyService _companyService;
        private readonly IEmailSender _emailService;
        private readonly UserManager<BTUser> _userManager;
       private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IBTInviteService _inviteService;   

        

        public InvitesController(ApplicationDbContext context, IBTProjectService projecctService, IBTCompanyService companyService, IEmailSender emailService, UserManager<BTUser> userManager, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration, IBTInviteService inviteService)
        {
            _context = context;
            _projectService = projecctService;
            _companyService = companyService;
            _emailService = emailService;
            _inviteService = inviteService; 
            _configuration = configuration;
            _userManager = userManager;
            _protector = dataProtectionProvider.CreateProtector(configuration.GetValue<string>("ProtectKey")! ?? Environment.GetEnvironmentVariable("ProtectKey")!);
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {

            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(company))
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_protector.Unprotect(token));
            string? inviteeEmail = _protector.Unprotect(email);
            int companyId = int.Parse(_protector.Unprotect(company));

            try
            {
                Invite? invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);

                if(invite != null)
                {

                    return View(invite);    
                }

                return NotFound();
            }
            catch (Exception)
            {

                throw;
            }


        }



        // GET: Invites/Create

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            int? companyId = User.Identity.GetCompanyId();
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.CompanyId == companyId), "Id", "Description");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InviteeEmail,InviteeFirstName,InviteeLastName,Message,ProjectId")] Invite invite)
        {


            int companyId = User.Identity.GetCompanyId();

            ModelState.Remove("InvitorId");

            if (ModelState.IsValid)
            {

                try
                {
                    Guid guid = Guid.NewGuid();

                    string? token = _protector.Protect(guid.ToString());
                    string? email = _protector.Protect(invite.InviteeEmail!);
                    string? company = _protector.Protect(companyId.ToString());

                    string? callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol: Request.Scheme);

                    string body = $@"{invite.Message} <br />
                    Please join my Company. <br />
                    Click the following link to join our team. <br />
                    <a href=""{callbackUrl}"">COLLABORATE</a>";

                    string? destination = invite.InviteeEmail;

                    Company btCompany = await _companyService.GetCompanyInfoAsync(companyId);

                    string? subject = $"BugSpy: {btCompany.Name}";

                    await _emailService.SendEmailAsync(destination!, subject, body);


                    //Save the INvite in the database 


                    invite.CompanyToken = guid;

                    invite.CompanyId = companyId;

                    invite.InviteDate = DataUtility.GetPostGresDate(DateTime.Now);

                    invite.InvitorId = _userManager.GetUserId(User);

                    invite.IsValid = true;  

                    await _inviteService.AddNewInviteAsync(invite);



                    return RedirectToAction(nameof(Index), "Home");

                    //TODO: sweet alert for email sent success/failure
                }
                catch (Exception)
                {

                    throw;
                }

    
           
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.CompanyId == companyId), "Id", "Description");
            return View(invite);
        }

        // GET: Invites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.FindAsync(id);
            if (invite == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", invite.CompanyId);
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", invite.ProjectId);
            return View(invite);
        }

        // POST: Invites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InviteDate,JoinDate,CompanyToken,InviteeEmail,InviteeFirstName,InviteeLastName,Message,IsValid,CompanyId,ProjectId,InvitorId,InviteeId")] Invite invite)
        {
            if (id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", invite.CompanyId);
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", invite.ProjectId);
            return View(invite);
        }

        // GET: Invites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // POST: Invites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Invites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Invites'  is null.");
            }
            var invite = await _context.Invites.FindAsync(id);
            if (invite != null)
            {
                _context.Invites.Remove(invite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int id)
        {
          return (_context.Invites?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
