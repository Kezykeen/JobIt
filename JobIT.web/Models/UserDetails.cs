using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobIT.web.Models
{
    [Bind(Exclude = "ProfilePicFile")]
    public class UserDetails
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Date Of Birth")]
        public System.DateTime? DateOfBirth { get; set; }

        [StringLength(70)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(40)]
        public string State { get; set; }

        [StringLength(40)]
        public string Country { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Select Picture")]
        public string ProfilePicPath { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public HttpPostedFileBase ProfilePicFile { get; set; }

        public virtual ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }
    }
}