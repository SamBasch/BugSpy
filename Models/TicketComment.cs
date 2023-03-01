using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comment")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Comment { get; set; }


        [DataType(DataType.Date)]
        public DateTime Created { get; set; }


        //FK start

        public int TicketId { get; set; }


        [Required]
        public string? UserId { get; set; }

        //FK end


        //TODO: setup navigation properties for 1 ticket and 1 user


        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? User { get; set; }    


    }
}
