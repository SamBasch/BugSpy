using BugSpy.Data;
using BugSpy.Models;
using BugSpy.Models.Enums;
using BugSpy.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BugSpy.Services
{
	public class BTNotificationService : IBTNotificationService

	{	private readonly ApplicationDbContext _context;

		private readonly IEmailSender _emailService;

		private readonly IBTRolesService _roleService;

		public BTNotificationService(ApplicationDbContext context, IEmailSender emailService, IBTRolesService roleService )
		{

			_context = context;
			_emailService = emailService;
			_roleService = roleService;
		}


		
		public async Task AddNotificationAsync(Notification? notfication)
		{
			try
			{
						
				if(notfication != null)
				{

					await _context.AddAsync(notfication);
					await _context.SaveChangesAsync();
				}


			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task AdminNotficationAsync(Notification? notification, int companyId)
		{
			try
			{
				if(notification != null)
				{

					IEnumerable<string> adminIds = (await _roleService.GetUserInRoleAsync(nameof(BTRoles.Admin), companyId))!.Select(u => u.Id);


					foreach(string adminId in adminIds)
					{
						notification.Id = 0;
						notification.RecipientId = adminId;

						await _context.AddAsync(notification);	

					}

					await _context.SaveChangesAsync();	
				}
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<Notification>> GetNotfificationByUserIdAsync(string? userId)
		{
			try
			{
				List<Notification> notifications = new();

				if (!string.IsNullOrEmpty(userId))
				{
					

					notifications = await _context.Notifications.Where(n => n.RecipientId == userId || n.SenderId == userId).Include(n => n.Recipient).Include(n => n.Sender).ToListAsync();
					  
				}

				return notifications;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> SendAdminEmailNotficiationAsync(Notification? notification, string? emailSubject, int companyId)
		{
			try
			{

				if(notification != null)
				{
					IEnumerable<string> adminEmails = (await _roleService.GetUserInRoleAsync(nameof(BTRoles.Admin), companyId))!.Select(u => u.Email);

					foreach (string adminEmail in adminEmails)
					{


						await _emailService.SendEmailAsync(adminEmail, emailSubject!, notification?.Message!);
						

					}
					return true;
				}



				return false;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> SendEmailNotficationAsync(Notification? notification, string? emailSubject)
		{
			try
			{

				if(notification != null)
				{

					BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

					string? userEmail = btUser?.Email;

					if (userEmail != null)
					{
						await _emailService.SendEmailAsync(userEmail, emailSubject!, notification.Message!);
						return true;

					}

					

				}

				return false;

			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
