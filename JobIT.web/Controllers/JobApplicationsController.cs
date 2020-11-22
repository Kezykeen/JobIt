using AutoMapper;
using JobIT.web.Models;
using JobIT.web.Respositories.JobApplicationsRepo;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Respositories.UserDetailsRepos;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobIT.web.Controllers
{
    [Authorize]
    public class JobApplicationsController : Controller
    {
        private readonly IJobApplicationsRepository _jobApplicationsRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IMapper _mapper;

        public JobApplicationsController(IJobApplicationsRepository jobApplicationRepo, IJobRepository jobRepository, IUserDetailsRepository userDetailsRepo, IMapper mapper)
        {
            _jobApplicationsRepository = jobApplicationRepo;
            _jobRepository = jobRepository;
            _userDetailsRepository = userDetailsRepo;
            _mapper = mapper;
        }

        // GET: JobApplications/Create
        public async Task<ActionResult> Create()
        {
            var userId = User.Identity.GetUserId();
            var currentUserDetails = await _userDetailsRepository.GetSingleAsync(x => x.UserId == userId);
            if (currentUserDetails == null)
            {
                return View();
            }
            
            return View(currentUserDetails);
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone,JobId,ApplicationStatusId,UserId")] UserDetails details, int jobId)
        {
            if (ModelState.IsValid)
            {
                var map = _mapper.Map<JobApplications>(details);
                map.UserId = User.Identity.GetUserId();
                var getJob = await _jobRepository.GetById(jobId);
                var getJobId = getJob.Id;
                map.JobId = getJobId;
                var created = await _jobApplicationsRepository.AddAsync(map);
                if (!created)
                {
                    return View(details);
                }
                return RedirectToAction("Index", "Job");
            }

            return View(details);
        }
    }
}