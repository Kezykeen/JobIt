 using JobIT.web.Models;
using JobIT.web.Respositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Respositories
{
    public class JobTypeRepository : GenericRepository<JobType>, IJobTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JobTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}