﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobIT.web.Models
{
    public class JobApplications
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        public virtual Job Job { get; set; }

        [ScaffoldColumn(false)]
        public int JobId { get; set; }

        public JobApplicationStatus? Status { get; set; } = JobApplicationStatus.Pending;

        [NotMapped]
        [ScaffoldColumn(false)]
        public HttpPostedFileBase ProfilePicFile { get; set; }

        [ScaffoldColumn(false)]
        public string ProfilePicPath { get; set; }

        [ScaffoldColumn(false)]
        public string ResumePath { get; set; }

        public virtual ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }
    }

    public enum JobApplicationStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}