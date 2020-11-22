using JobIT.web.Models;
using JobIT.web.Respositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Respositories.JobApplicationsRepo
{
    public class JobApplicationsRepository : GenericRepository<JobApplications>, IJobApplicationsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JobApplicationsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}