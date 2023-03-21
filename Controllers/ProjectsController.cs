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
using X.PagedList;
using System.Drawing.Printing;
using BugSpy.Extensions;
using BugSpy.Models.ViewModels;
using BugSpy.Models.Enums;
using System.Collections;

namespace BugSpy.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {

       


        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _fileService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _roleService;
        private readonly IBTProjectService _btProjectService;  

        public ProjectsController(ApplicationDbContext context, IBTFileService fileService, UserManager<BTUser> userManager, IBTProjectService btProjectService, IBTRolesService roleService)
        {
            _context = context;
            _fileService = fileService; 
            _userManager = userManager;
            _btProjectService = btProjectService;   
            _roleService = roleService;
            
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


            int companyId = User.Identity!.GetCompanyId();

   


            string? userId = _userManager.GetUserId(User);




                        


                IEnumerable<Project> myRecentProjects = (await _btProjectService.GetSignedInUserProjects(companyId, userId));
            return View(myRecentProjects);





        }



        public async Task<IActionResult> ProjectsIndex()



        {



            int companyId = User.Identity!.GetCompanyId();

       


            string? userId = _userManager.GetUserId(User);

      

        
                IEnumerable<Project> companyProjects = (await _btProjectService.GetAllCompanyProjects(companyId));
                return View(companyProjects);
            



         





        }

        public async Task<IActionResult> CompanyArchivedProjects()



        {



            int companyId = User.Identity!.GetCompanyId();



            string? userId = _userManager.GetUserId(User);





            IEnumerable<Project> companyProjects = (await _btProjectService.GetAllCompanyArchivedProjects(companyId));
            return View(companyProjects);










        }

        public async Task<IActionResult> ProjectOverview(int? id)
        {
            BTUser? loggedInUser = await _userManager.GetUserAsync(User);

            string? userId = _userManager.GetUserId(User);

            int companyId = loggedInUser!.CompanyId;

            Project? project = await _context.Projects.Include(p => p.Members).Include(p => p.Tickets).Include(p => p.Company).Include(p => p.ProjectPriority).FirstOrDefaultAsync(p => p.Id == id.Value && p.CompanyId == companyId);

            return View(project);


        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();

            IEnumerable<BTUser> projectManagers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.ProjectManager), companyId);

            BTUser? currentPM = await _btProjectService.GetProjectManagerAsync(Id);

            AssignPMViewModel viewModel = new()
            {

                Project = await _btProjectService.GetProjectByIdAsync(companyId, Id),
                PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id),
                PMId = currentPM?.Id


            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(AssignPMViewModel viewModel)
        {

            if(!string.IsNullOrEmpty(viewModel.PMId))
            {
                await _btProjectService.AddProjectManagerAsync(viewModel.PMId, viewModel.Project?.Id);

                return RedirectToAction("Details", new { id = viewModel.Project?.Id });
            }


            ModelState.AddModelError("PMID", "No Manager chosen. Please Select a PM");

            int companyId = User.Identity.GetCompanyId();

            IEnumerable<BTUser> projectManagers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.ProjectManager), companyId);

            BTUser? currentPM = await _btProjectService.GetProjectManagerAsync(viewModel.Project.Id);

            viewModel.Project = await _btProjectService.GetProjectByIdAsync(companyId, viewModel.Project.Id);

            viewModel.PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id);

            viewModel.PMId = currentPM?.Id;

            return View(viewModel);



        }




        [HttpGet]
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> AssignProjectMembers(int? id)
        {
            if(id == null)
            {

                return NotFound();

            }

            int companyId = User.Identity.GetCompanyId();


            Project project = await _btProjectService.GetProjectByIdAsync(companyId, id);

            List<BTUser> submitters = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Developer), companyId);

            List<BTUser> usersList = submitters.Concat(developers).ToList();

            List<string> currentMembers = project.Members.Select(m => m.Id).ToList();

            ProjectMembersViewModel viewModel = new()
            {
                Project = await _btProjectService.GetProjectByIdAsync(companyId, id),


                UsersList = new MultiSelectList(usersList, "Id", "FullName", currentMembers)


            };

            return View(viewModel); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignProjectMembers(ProjectMembersViewModel viewModel)
        {


            int companyId = User.Identity.GetCompanyId();


            if(viewModel.SelectedMembers != null)
            {

                await _btProjectService.RemoveAllMembersFromProject(viewModel.Project!.Id, companyId);


                await _btProjectService.AddMembersToProject(viewModel.SelectedMembers, viewModel.Project!.Id, companyId);

                return RedirectToAction(nameof(Details), new { id = viewModel.Project.Id });
            }

            ModelState.AddModelError("SelectedMembers", "No Users chosen. Please select users");
            viewModel.Project = await _btProjectService.GetProjectByIdAsync(companyId, viewModel.Project!.Id);
            List<string> currentMembers = viewModel.Project.Members.Select(m => m.Id).ToList();
            List<BTUser> submitters = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _roleService.GetUserInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> usersList = submitters.Concat(developers).ToList();

            viewModel.UsersList = new MultiSelectList(usersList, "Id", "FullName", currentMembers);

            return View(viewModel); 
        }







        // GET: Projects/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            string? userId = _userManager.GetUserId(User);


            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, id);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }









        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {


            int companyId = User.Identity!.GetCompanyId();

            ViewData["ProjectPriorityId"] = new SelectList( await _btProjectService.GetProjectPriorityList(), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Created,StartDate,EndDate,ImageFormFile,Archived,Members,CompanyId,ProjectPriorityId")] Project project)
        {

        

            int companyId = User.Identity!.GetCompanyId();

            project.CompanyId = companyId;

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


            

                return RedirectToAction(nameof(ProjectsIndex));


            }

            ViewData["ProjectPriorityId"] = new SelectList(await _btProjectService.GetProjectPriorityList(), "Id", "Name");
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

            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser> Members = await _context.Users.Include(u => u.Projects).Where(u => u.CompanyId == companyId).ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, id);
            if (project == null)
            {
                return NotFound();
            }



            ViewData["ProjectPriorityId"] = new SelectList(await _btProjectService.GetProjectPriorityList(), "Id", "Name");
            return View(project);
        }
        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,StartDate,EndDate,ImageFormFile,Archived,CompanyId,ProjectPriorityId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            project.CompanyId = companyId;

            if (ModelState.IsValid)
            {
                try
                {
                    if (project.ImageFormFile != null)
                    {
                        project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                        project.ImageFileType = project.ImageFormFile.ContentType;
                    }


                    project.Created = DataUtility.GetPostGresDate(project.Created);
                    project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                    project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);
                    _context.Update(project);
                    await _context.SaveChangesAsync();


                    if (project.Archived == true)
                    {
                       
                       
                        await _btProjectService.ArchiveProjectTickets(project);

                        await _context.SaveChangesAsync();





                    }


           


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
                return RedirectToAction(nameof(ProjectsIndex));
            }

            ViewData["ProjectPriorityId"] = new SelectList(await _btProjectService.GetProjectPriorityList(), "Id", "Name");
            return View(project);
        }


        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {

            int companyId = User.Identity!.GetCompanyId();

            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? Id)
        {
            int companyId = User.Identity!.GetCompanyId();


            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, Id);


            if (project != null)
            {
                project.Archived = true;
                _context.Update(project);
                await _btProjectService.ArchiveProjectTickets(project);

                await _context.SaveChangesAsync();

             
                


            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }









        // POST: Projects/Delete/5
        [HttpPost, ActionName("Unarchive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnarchiveConfirm(int? Id)
        {

            int companyId = User.Identity!.GetCompanyId();

            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, Id);


            if (project != null)
            {
                project.Archived = false;
                _context.Update(project);
              

                await _context.SaveChangesAsync();





            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("UnarchiveWithTickets")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnarchiveAndTicketsConfirm(int? Id)
        {

            int companyId = User.Identity!.GetCompanyId();

            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            Project? project = await _btProjectService.GetProjectByIdAsync(companyId, Id);


            if (project != null)
            {
                project.Archived = false;
                _context.Update(project);
                await _btProjectService.UnarchiveProjectTickets(project);

                await _context.SaveChangesAsync();





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
