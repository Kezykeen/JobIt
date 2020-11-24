using AutoMapper;
using JobIT.web.Dto;
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
            result = result.Include("JobApplications");;
            
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
                return View(model);
            }

            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name", model.JobTypeId);
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name", model.SectorId);
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
                return RedirectToAction("Index");
            }

            ViewBag.JobTypeId = new SelectList(_jobTypeRepository.ToList(), "Id", "Name", job.JobTypeId);
            ViewBag.SectorId = new SelectList(_sectorRepository.ToList(), "Id", "Name", job.SectorId);
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
        public async Task<ActionResult> ViewApplication([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone,JobId,Status,UserId")] JobApplications jobApplications, int? id)
        {
            if (ModelState.IsValid)
            {
                await _jobApplicationsRepository.Update(jobApplications);
                await _jobApplicationsRepository.Save();

                var userId = User.Identity.GetUserId();
                var currentUserDetails = await _userDetailsRepository.GetSingleAsync(x => x.UserId == userId);
                JobApplications getJob = await _jobApplicationsRepository.GetById(id);

                if (getJob != null)
                {
                    bool changed = getJob.Status != jobApplications.Status;
                    if (changed)
                    {
                        if (currentUserDetails != null)
                        {
                            await _sendMailNotification.SendMail("kezykeen@gmail.com", "Your job application has been reviewed, visit your dashboard for more details", "Job application notification");
                        }
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
                string[] extension = file.Split('.');
                return File(file, extension[1]);
            }
            else
            {
                return HttpNotFound();
            }

        }
    }
}