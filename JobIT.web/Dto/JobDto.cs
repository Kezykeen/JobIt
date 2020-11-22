using AutoMapper;
using JobIT.web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobIT.web.Dto
{
    public class JobDto
    {
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        public string Description { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Responsibilities required")]
        public string Responsibilities { get; set; }
        public string Salary { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Location required")]
        public string Location { get; set; }
    }

    public class CreateJobDto
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Responsibilities { get; set; }

        public string Salary { get; set; }

        public string Location { get; set; }

        public string Company { get; set; }

        public int JobTypeId { get; set; }

        public int SectorId { get; set; }
    }
    public class UpdateJobDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Responsibilities { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public int JobTypeId { get; set; }
        public int SectorId { get; set; }
        public bool Status { get; set; }
    }

    public class UpdateJobApplicantDto
    {
        [Required]
        public int JobId { get; set; }
        [Required]
        public int ApplicantId { get; set; }
        public bool Status { get; set; }
    }
}