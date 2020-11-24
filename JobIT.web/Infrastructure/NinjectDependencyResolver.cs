using JobIT.web.Models;
using JobIT.web.Respositories.Generic;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobIT.web
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            //kernel.Bind<IGenericRepository<>>().To<GenericRepository<>>();
            //kernel.Bind<IJobRepository>().To<JobRepository>();

        }
    }
}