using AutoMapper;
using JobIT.web.Dto;
using JobIT.web.Extensions;
using JobIT.web.Models;
using JobIT.web.Respositories;
using JobIT.web.Respositories.JobApplicationsRepo;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Respositories.SectorRepos;
using JobIT.web.Respositories.UserDetailsRepos;
using JobIT.web.Services;
using Microsoft.AspNet.Identity;
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
    [Authorize(Roles = RoleName.Admin)]
    public class AdminController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobApplicationsRepository _jobApplicationsRepository;
        private readonly IJobTypeRepository _jobTypeRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IMapper _mapper;
        private readonly ISendMailNotification _sendMailNotification;

        public AdminController(IJobRepository jobRepository, IJobApplicationsRepository jobApplicationsRepository, IJobTypeRepository jobTypeRepository, ISectorRepository sectorRepository, IUserDetailsRepository userDetailsRepo, ISendMailNotification sendMailNotification, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _jobApplicationsRepository = jobApplicationsRepository;
            _jobTypeRepository = jobTypeRepository;
            _sectorRepository = sectorRepository;
            _userDetailsRepository = userDetailsRepo;
            _sendMailNotification = sendMailNotification;
            _mapper = mapper;
        }

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            var result = await _jobRepository.Get(null);
            result = result.Include("JobApplications"); ;

            return View(result);
        }

        public ActionResult CreateJob()
        {
            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name");
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name");
            return View(new CreateJobDto());
        }

        [HttpPost]
        public async Task<ActionResult> CreateJob(CreateJobDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var map = _mapper.Map<Job>(model);
            map.CreatedBy = User.Identity.GetUserName();
            var created = await _jobRepository.AddAsync(map);
            if (!created)
            {
                this.AddNotification("Job creation failed", NotificationType.ERROR); // error alert for failed job creation
                return View(model);
            }

            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name", model.JobTypeId);
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name", model.SectorId);

            this.AddNotification("Job Created Successfully", NotificationType.SUCCESS); //success alert for job creation
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> JobDetails(int Id)
        {
            var currentJob = await _jobRepository.GetById(Id);
            if (currentJob == null)
            {
                return HttpNotFound();
            }
            return View(currentJob);
        }

        // GET: Job/Edit/5
        public async Task<ActionResult> EditJob(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = await _jobRepository.GetById(id);
            if (job == null)
            {
                return HttpNotFound();
            }

            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name", job.JobTypeId);
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name", job.SectorId);
            return View(job);
        }

        // POST: Job/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditJob([Bind(Include = "Id,Title,Description,Responsibilities,Salary,Location,Company,JobTypeId,SectorId,Status,CreatedDate,CreatedBy")] Job job)
        {
            if (ModelState.IsValid)
            {
                await _jobRepository.Update(job);
                await _jobRepository.Save();

                this.AddNotification("Job updated successfully", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name", job.JobTypeId);
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name", job.SectorId);

            this.AddNotification("Job update failed", NotificationType.ERROR);
            return View(job);
        }

        public async Task<ActionResult> DeleteJob(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = await _jobRepository.GetById(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Job/DeleteJob/5
        [HttpPost, ActionName("DeleteJob")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Job job = await _jobRepository.GetById(id);
            await _jobRepository.Delete(job);
            await _jobRepository.Save();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ApplicationsAccepted()
        {
            var getApplications = await _jobApplicationsRepository.Get(null);
            var applicationssAccepted = getApplications.Where(c => c.Status.ToString() == "Accepted").Include("Job");

            TempData["acceptedCount"] = applicationssAccepted.ToArray().Count();

            return View(applicationssAccepted);
        }

        public async Task<ActionResult> ApplicationsRejected()
        {
            var getApplications = await _jobApplicationsRepository.Get(null);
            var applicationsRejected = getApplications.Where(c => c.Status.ToString() == "Rejected").Include("Job");

            TempData["rejectedCount"] = applicationsRejected.ToArray().Count();

            return View(applicationsRejected);
        }

        public async Task<ActionResult> ApplicationsUnattended()
        {
            var getApplications = await _jobApplicationsRepository.Get(null);
            var applicationsUnattended = getApplications.Where(c => c.Status.ToString() == "Pending").Include("Job");

            TempData["unattendedCount"] = applicationsUnattended.ToArray().Count();

            return View(applicationsUnattended);
        }

        public async Task<ActionResult> TotalApplications()
        {
            var getApplications = await _jobApplicationsRepository.Get(null);
            getApplications = getApplications.Include("Job");

            TempData["applicationsCount"] = getApplications.ToArray().Count();

            return View(getApplications);
        }

        public async Task<ActionResult> TotalUsers()
        {
            var getUsers = await _userDetailsRepository.Get(null);

            TempData["usersCount"] = getUsers.ToArray().Count();

            return View(getUsers);
        }

        // GET: JobApplications/Edit/5
        public async Task<ActionResult> ViewApplication(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobApplications jobApplications = await _jobApplicationsRepository.GetById(id);
            if (jobApplications == null)
            {
                return HttpNotFound();
            }
            return View(jobApplications);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ViewApplication([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone,JobId,Status,UserId,ProfilePicPath,ResumePath")] JobApplications jobApplications)
        {
            if (ModelState.IsValid)
            {
                JobApplications getJobApplication = await _jobApplicationsRepository.GetById(jobApplications.Id);

                if (getJobApplication != null)
                {
                    bool changed = getJobApplication.Status != jobApplications.Status;
                    if (changed)
                    {
                        var jobAppliedFor = await _jobRepository.GetSingleAsync(c => c.Id == jobApplications.JobId);

                        if (jobApplications.Status.ToString() == "Accepted")
                        {
                            var mail = $"Congratulations!" + $" Your application for {jobAppliedFor.Title} at {jobAppliedFor.Company} has been accepted." + $" We will get to you very soon with details of your employment.";
                            await _sendMailNotification.SendMail(jobApplications.Email, mail, "Job Application Has Been Reviewed");
                        }
                        else
                        {
                            var mail = $"Your application for {jobAppliedFor.Title} at {jobAppliedFor.Company} has been rejected." + $" Don't lose hope though, with our ever growing list of jobs, you can be sure to never run out of possible jobs.";
                            await _sendMailNotification.SendMail(jobApplications.Email, mail, "Job Application Has Been Reviewed");
                        }
                        getJobApplication.Status = jobApplications.Status;
                        await _jobApplicationsRepository.Update(getJobApplication);
                        this.AddNotification("Application Review successfully", NotificationType.SUCCESS);
                    }
                }



                return RedirectToAction("Index");
            }

            return View(jobApplications);
        }


        [HttpGet]
        public async Task<ActionResult> ViewJobApplications(int Id)
        {
            var applications = await _jobApplicationsRepository.Get(c => c.JobId == Id);
            applications = applications.Include("Job");
            return View(applications);
        }

        public async Task<ActionResult> DownloadFile(int Id)
        {
            var file = "";
            JobApplications getJobApplication = await _jobApplicationsRepository.GetById(Id);
            if (getJobApplication != null)
            {
                file = getJobApplication.ResumePath;
                var fileSplit = file.Split('/');
                return File(file, "application/" + file.Split('.')[1], fileSplit[fileSplit.Length - 1]);
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}