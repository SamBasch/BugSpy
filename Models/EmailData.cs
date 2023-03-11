using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace BugSpy.Models
{
    public class EmailData


       
    {

        [Required]
        public string? EmailAddress { get; set; }

        [Required]
        public string? EmailBody { get; set; }

        [Required]
        public string? Name { get; set; }


        public string? EmailSubject { get; set; }   



    





    }
}
