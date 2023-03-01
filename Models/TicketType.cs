using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class TicketType
    {

        public int Id { get; set; }




      
        [Display(Name = "Ticket Type")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }


       
    }
}
