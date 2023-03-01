using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugSpy.Models
{
    public class Company
    {

        public int Id { get; set; }

        
        [Required]
        [Display(Name = "Company Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }


        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }


        //TODO set up navigation for multiple projects, multiple members(BTUSER), and multiple invites

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }
}