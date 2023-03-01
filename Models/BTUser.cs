﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugSpy.Models
{
    public class BTUser : IdentityUser
    {

        [Required]
        [Display(Name = "First Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? FirstName { get; set; }




        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? LastName { get; set; }


        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        public int CompanyId { get; set; }

        public byte[]? ImageFileData { get; set; }

        public string? ImageFileType { get; set; }


        [NotMapped]
        public virtual IFormFile? ImageFormFile { get; set; }


       //TODO: Set up navigational properties for 1 company and a collection or projects

        public virtual Company? Company { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();



    }
}
