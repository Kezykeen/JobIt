using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Models
{
    public class JobApplicationUserDetailsViewModel
    {
        public JobApplications JobApplication { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}