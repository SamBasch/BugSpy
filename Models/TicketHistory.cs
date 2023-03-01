using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class TicketHistory
    {

        public int Id { get; set; }



        [Display(Name = "Property Changed")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? PropertyName { get; set; }


        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }



        [Display(Name = "Old Value")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? OldValue { get; set; }


        [Display(Name = "New Value")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? NewValue { get; set; }




        //FK start

        public int TicketId { get; set; }   

        public string? UserId { get; set; }  




        //Fk End



        //TODO set up navigation properties for 1 ticket and 1 user


        public virtual Ticket? Ticket { get; set; }  

        public virtual BTUser? User { get; set; }    
    }
}
