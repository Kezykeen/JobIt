using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobIT.web.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Job title required")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Responsibilities required")]
        public string Responsibilities { get; set; }

        public string Salary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Location required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Company name required")]
        public string Company { get; set; }

        public JobType JobType { get; set; }

        public int JobTypeId { get; set; }

        public Sector Sector { get; set; }

        public int SectorId { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }
        
        public ICollection<JobApplications> JobApplications { get; set; }
    }
}