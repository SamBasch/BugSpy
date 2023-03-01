using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugSpy.Models
{
    public class Project
    {

        public int Id { get; set; }


       
        [Required]
        [Display(Name = "Project Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }

 
        [Required]
        [Display(Name = "Description")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }


        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }


        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public byte[]? ImageFileData { get; set; }

        public string? ImageFileType { get; set; }

        [NotMapped]
        public virtual IFormFile? ImageFormFile { get; set; }


        public bool Archived { get; set; }

        //FK start

        public int CompanyId { get; set; }

        public int ProjectPriorityId { get; set; }

        //Fk end

        //TODO: setup navigation properties for 1 company, 1 projectpriorty, collectin of members, collection of tickets


        public virtual Company? Company { get; set; }

        public virtual ProjectPriority? ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();


    }
}
