using JobIT.web.Models;
using JobIT.web.Respositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Respositories.SectorRepos
{
    public class SectorRepository : GenericRepository<Sector>, ISectorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SectorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}