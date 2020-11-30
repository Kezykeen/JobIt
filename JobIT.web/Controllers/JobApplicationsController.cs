using AutoMapper;
using JobIT.web.Extensions;
using JobIT.web.Models;
using JobIT.web.Respositories.JobApplicationsRepo;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Respositories.UserDetailsRepos;
using JobIT.web.Services;
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
        private readonly ISendMailNotification _sendMailNotification;
        private readonly IMapper _mapper;

        public JobApplicationsController(IJobApplicationsRepository jobApplicationRepo, IJobRepository jobRepository, IUserDetailsRepository userDetailsRepo, ISendMailNotification sendMailNotification, IMapper mapper)
        {
            _jobApplicationsRepository = jobApplicationRepo;
            _jobRepository = jobRepository;
            _userDetailsRepository = userDetailsRepo;
            _sendMailNotification = sendMailNotification;
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
                var userId = User.Identity.GetUserId();

                map.UserId = userId;

                var getJob = await _jobRepository.GetById(jobId);
                var getJobId = getJob.Id;

                map.JobId = getJobId;

                var getUser = await _userDetailsRepository.GetSingleAsync(c => c.UserId == userId);

                map.ResumePath = getUser.ResumePath;
                map.ProfilePicPath = getUser.ProfilePicPath;

                var created = await _jobApplicationsRepository.AddAsync(map);

                if (!created)
                {
                    this.AddNotification("Application Failed", NotificationType.ERROR); //error alert for failed job application
                    return View(details);
                }


                //Send mail notification to the user after successful application
                var currentUserDetails = await _userDetailsRepository.GetSingleAsync(x => x.UserId == userId);
                if (currentUserDetails != null)
                {
                    var mail = $"Your application was successful," + $" We will get back to you with details of our feedback. Ensure to frequent your dashboard.";
                    await _sendMailNotification.SendMail(currentUserDetails.Email, mail, "Job application notification");
                }

                this.AddNotification("Application Successful", NotificationType.SUCCESS); //success alert for successful job application

                return RedirectToAction("Index", "Job");

            }

            return View(details);
        }
    }
}