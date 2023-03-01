using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class Invite
    {

        public int Id { get; set; }



        [DataType(DataType.Date)]
        public DateTime InviteDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }


        [Display(Name = "Company Token")]
        public Guid CompanyToken { get; set; }



        [Required]
        [Display(Name = "Invitee Email")]
        public string? InviteeEmail { get; set; }

        
        [Required]
        [Display(Name = "Invitee First Name")]
        public string? InviteeFirstName { get; set; }


        [Required]
        [Display(Name = "Invitee Last Name")]
        public string? InviteeLastName { get; set; }


        [Display(Name = "Invite Message")]
        public string? Message { get; set; }


        [Display(Name = "IsValid?")]
        public bool IsValid { get; set; }

        //FK start

        public int CompanyId { get; set; }  

        public int ProjectId { get; set; }


        [Required]
        public string? InvitorId { get; set;}


        public string? InviteeId { get; set; }

        //Fk end

        //TODO: setup navigation properties for 1 company, 1 project, 1 invitor, 1 invitee

        public virtual Company? Company { get; set; }

        public virtual Project? Project { get; set; }    

        public virtual BTUser? Invitor { get; set; } 

        public virtual BTUser? Invitee { get; set; } 


    }
}
