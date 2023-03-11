using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugSpy.Services
{
    public class BTCompanyService : IBTCompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public BTCompanyService(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
       

        }



        public async Task<Company> GetCompanyInfoAsync(int? companyId)
        {

            try
            {
                Company? company = await _context.Companies.Include(c => c.Projects).Include(c => c.Members).Include(c => c.Invites).FirstOrDefaultAsync(c => c.Id == companyId);

                return company!;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<BTUser>> GetMembersAsync(int? companyId)
        {


            try
            {
                List<BTUser> users = await _context.Users.Where(u => u.CompanyId == companyId).Include(u => u.Company).Include(u => u.Projects).ToListAsync();
                return users;
            }
            catch (Exception)
            {

                throw;
            }


  
        }
    }
}
