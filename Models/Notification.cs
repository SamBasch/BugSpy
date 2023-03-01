using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class Notification
    {
        public int Id { get; set; }


        
        [Required]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }


        
        [Required]
        [Display(Name = "Message")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [Display(Name = "Viewed?")]
        public bool HasBeenViewed { get; set; }

        //Fk start

        
        public int ProjectId { get; set; }


        
        public int TicketId { get; set; }


      
        public int NotificationTypeId { get; set; }

        
        [Required]
     
        public string? SenderId { get; set; }



        [Required]
        
        public string? RecipientId { get; set; } 

        //fk end



        //TODO: setup navigation properties for 1 notification type, 1 a particular ticket,  1 particular project,  1 from a particular sender,  1to a particular recipient


        public virtual NotificationType? NotificationType { get; set; }

        public virtual Ticket? Ticket { get; set; }

        public virtual Project? Project { get; set; }

        public virtual BTUser? Sender { get; set; }

        public virtual BTUser? Recipient { get; set; }   

    }
}
