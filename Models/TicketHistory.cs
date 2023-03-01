using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class TicketHistory
    {

        public int Id { get; set; }




        public string? PropertyName { get; set; }


        [Display(Name = "Description")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }



        [Display(Name = "Old Value")]

        public string? OldValue { get; set; }


        [Display(Name = "New Value")]
 
        public string? NewValue { get; set; }




        //FK start

        public int TicketId { get; set; }


        [Required]
        public string? UserId { get; set; }  




        //Fk End



        //TODO set up navigation properties for 1 ticket and 1 user


        public virtual Ticket? Ticket { get; set; }  

        public virtual BTUser? User { get; set; }    
    }
}
