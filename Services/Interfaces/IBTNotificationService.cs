using BugSpy.Models;

namespace BugSpy.Services.Interfaces
{
	public interface IBTNotificationService
	{

		public Task AddNotificationAsync(Notification? notfication);


		public Task AdminNotficationAsync(Notification? notification, int companyId);


		public Task<List<Notification>> GetNotfificationByUserIdAsync(string? userId);


		public Task<bool> SendAdminEmailNotficiationAsync(Notification? notification, string? emailSubject, int companyId);


		public Task<bool> SendEmailNotficationAsync(Notification? notification, string? emailSubject);
	}
}
