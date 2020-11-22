using AutoMapper;
using JobIT.web.Models;
using JobIT.web.Respositories;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Respositories.SectorRepos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobIT.web.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // GET: Job
        public async Task<ActionResult> Index()
        {
            var result = await _jobRepository.Get(x => x.Status == true);
            result = result.Include("JobType").Include("Sector");
            return View(result);
        }

        // GET: Job/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var job = await _jobRepository.Get(x => x.Id == id);
            var jobDetails = job.Include("JobType").Include("Sector").FirstOrDefault();

            if (jobDetails == null)
            {
                return HttpNotFound();
            }
            return View(jobDetails);
        }
    }
}