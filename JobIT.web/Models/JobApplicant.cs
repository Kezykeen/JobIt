using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Models
{
    public class JobApplicant
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public bool? Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}