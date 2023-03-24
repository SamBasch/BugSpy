using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Models.ViewModels;
using BugSpy.Extensions;
using Microsoft.Build.Evaluation;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using BugSpy.Models.Enums;
using System.Runtime.InteropServices;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BugSpy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyService _companyService;
        public CompaniesController(ApplicationDbContext context, IBTRolesService roleService, UserManager<BTUser> userManager, IBTCompanyService companyService)
        {
            _context = context;
            _roleService = roleService;
            _userManager = userManager;
            _companyService = companyService;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
              return _context.Companies != null ? 
                          View(await _context.Companies.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {


            List<ManageUserRolesViewModel> viewModels = new()
            {


            };

            int companyId = User.Identity.GetCompanyId();

            List<BTUser> users = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            List<IdentityRole> roles = await _roleService.GetRolesAsync();


            foreach (BTUser user in users)
            {


                

                List<string>? currentRoles = (await _roleService.GetUserRolesAsync(user)).ToList();





                ManageUserRolesViewModel viewmodel = new()
                {


                    BTUser = user,



                    Roles = new MultiSelectList(roles, "Name", "Name", await _roleService.GetUserRolesAsync(user)),

                    



                };

                viewModels.Add(viewmodel);

            }

            return View(viewModels);    
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel viewModel)
        {
            int companyId = User.Identity.GetCompanyId();




            BTUser? Btuser = await _userManager.FindByIdAsync(viewModel.BTUser!.Id);

            if (Btuser == null)
            {

                return NotFound();
            }


            IEnumerable<string>? currentRoles = viewModel.SelectedRoles;



          await _roleService.RemoveUserFromRolesAsync(Btuser, await _roleService.GetUserRolesAsync(viewModel.BTUser));

            foreach(string role in currentRoles)
            {
                await _roleService.AddUserToRoleAsync(Btuser, role);
            }


           return RedirectToAction("ManageUserRoles");
           
     

            


        }



        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            Company? company = await _companyService.GetCompanyInfoAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
