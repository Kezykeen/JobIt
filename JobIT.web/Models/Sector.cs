using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Job> Jobs { get; set; }
    }
}