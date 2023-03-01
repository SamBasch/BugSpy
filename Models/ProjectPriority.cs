using System.ComponentModel.DataAnnotations;

namespace BugSpy.Models
{
    public class ProjectPriority
    {


        public int Id { get; set; }

     
        [Required]
        [Display(Name = "Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }



        
    }
}
