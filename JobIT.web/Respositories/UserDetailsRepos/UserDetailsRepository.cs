using JobIT.web.Models;
using JobIT.web.Respositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web.Respositories.UserDetailsRepos
{
    public class UserDetailsRepository : GenericRepository<UserDetails>, IUserDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}