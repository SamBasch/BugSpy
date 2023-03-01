using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class Ticket
    {

        public int Id { get; set; }

     
        [Required]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }

   
        [Required]
        [Display(Name = "Description")]
        [StringLength(6000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }


        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }


        public bool Archived { get; set; }

        public bool ArchivedByProject { get; set; }

        //FK start

        public int ProjectId { get; set; }  

        public int TicketTypeId { get; set; }

        public int TicketStatusId { get; set; } 

        public int TicketPriorityId { get; set; }   

        public string? DeveloperUserId { get; set; }


        [Required]
        public string? SubmitterUserId { get; set; } 

        //FKend



        //navigation 

        public virtual Project? Project { get; set; }

        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }  


        public virtual BTUser? DeveloperUser { get; set; }   

        public virtual BTUser? SubmitterUser { get; set; }


        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();


        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
    }
}
