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
using System.Net.Sockets;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Authorization;
using BugSpy.Services;

namespace BugSpy.Controllers
{

    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _fileService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTService _btService;

        public ProjectsController(ApplicationDbContext context, IBTFileService fileService, UserManager<BTUser> userManager, IBTService btService)
        {
            _context = context;
            _fileService = fileService; 
            _userManager = userManager;
            _btService = btService;
            
        }

        // GET: Projects

        public async Task<IActionResult> Index()
        {

            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            int companyId = loggedInUser!.CompanyId;

            IEnumerable<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();
            return View(projects);
        }


        //GET: Projects Overview

        public async Task<IActionResult> ProjectsOverview()
        {
            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            string? userId = _userManager.GetUserId(User);

            int companyId = loggedInUser!.CompanyId;

            IEnumerable<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Members.Any(m => m.Id == userId)).Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).ToListAsync();
            return View(projects);


        }



        public async Task<IActionResult> ProjectOverview(int? id)
        {
            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            string? userId = _userManager.GetUserId(User);

            int companyId = loggedInUser!.CompanyId;

            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value && p.CompanyId == companyId);

            return View(project);


        }





        // GET: Projects/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }









        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            int companyId = loggedInUser!.CompanyId;

            IEnumerable<BTUser> Members = await  _context.Users.Include(u => u.Projects).Where(u => u.CompanyId == companyId).ToListAsync();


            ViewData["Members"] = new MultiSelectList(_context.Users.Where(u => u.CompanyId == companyId).Include(u => u.Projects), "Id", "FullName");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Created,StartDate,EndDate,ImageFormFile,Archived,Members,CompanyId,ProjectPriorityId")] Project project, IList<string> selected)
        {

            //BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            //int companyId = loggedInUser!.CompanyId;

            if (ModelState.IsValid)
            {
                project.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);


                if (project.ImageFormFile != null)
                {
                    project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                    project.ImageFileType = project.ImageFormFile.ContentType;
                }


                project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);


                _context.Add(project);
                await _context.SaveChangesAsync();


                await _btService.MembersToProject(selected, project.Id);

                return RedirectToAction(nameof(Index));


            }

            //ViewData["Members"] = new MultiSelectList(_context.Users.Where(u => u.CompanyId == companyId).Include(u => u.Projects), "Id", "FullName");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }


















        //// GET: Projects/Create
        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{

        //    BTUser? loggedInUser = await _userManager.GetUserAsync(User);

        //    int companyId = loggedInUser!.CompanyId;

        //    IEnumerable<BTUser> Members = await _context.Users.Include(u => u.Projects).Where(u => u.CompanyId == companyId).ToListAsync();

        //    ViewData["Members"] = new MultiSelectList(Members, "Id", "FullName");
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
        //    ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name");
        //    return View();
        //}

        //// POST: Projects/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Description,Created,StartDate,EndDate,ImageFormFile,Archived,CompanyId,ProjectPriorityId")] Project project, IEnumerable<int> Selected)
        //{

        //    ModelState.Remove("Created");
        //    ModelState.Remove("ImageFileData");
        //    ModelState.Remove("ImageFileType");

        //    if (ModelState.IsValid)
        //    {
        //        project.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);


        //        if (project.ImageFormFile != null)
        //        {
        //            project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
        //            project.ImageFileType = project.ImageFormFile.ContentType;
        //        }


        //            project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
        //            project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);


        //        _context.Add(project);
        //        await _context.SaveChangesAsync();

        //        await _btService.MembersToProject(Selected, project.Id);




        //    }


        //    return View(project);
        //}


        // GET: Projects/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,StartDate,EndDate,ImageFormFile,Archived,CompanyId,ProjectPriorityId")] Project project, IList<string> selected)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.Created = DataUtility.GetPostGresDate(project.Created);
                    project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                    project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);
                    _context.Update(project);
                    await _context.SaveChangesAsync();

                    

                    if(selected != null)
                    {
                        await _btService.RemoveAllMembersFromProject(project.Id);


                        await _btService.MembersToProject(selected, project.Id);

                    }
                    _context.Update(project);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value);
            if (project != null)
            {
                project.Archived = true;
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
