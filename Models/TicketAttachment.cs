using BugSpy.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugSpy.Models
{
    public class TicketAttachment
    {

        public int Id { get; set; }


        [Required]
        [Display(Name = "Description")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }





        public byte[]? FileData { get; set; }

        public string? FileType { get; set; }


        [NotMapped]
		[DisplayName("Select a file")]
		[DataType(DataType.Upload)]
		[MaxFileSize(1024 * 1024)]
		[AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
		public virtual IFormFile? FormFile { get; set; }




        //FK start 

        public int TicketId { get; set; }

        [Required]
        public string? BTUserId { get; set; }    

        //Fk End


        //TODO: setup navigation properties for 1 ticket and 1 user

        public virtual Ticket? Ticket { get; set; }


        public virtual BTUser? BTUser { get; set; }    



    }
}
