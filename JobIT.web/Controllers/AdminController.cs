using AutoMapper;
using JobIT.web.Dto;
using JobIT.web.Models;
using JobIT.web.Respositories;
using JobIT.web.Respositories.JobApplicationsRepo;
using JobIT.web.Respositories.JobRepos;
using JobIT.web.Respositories.SectorRepos;
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
        private readonly IMapper _mapper;

        public AdminController(IJobRepository jobRepository, IJobApplicationsRepository jobApplicationsRepository, IJobTypeRepository jobTypeRepository, ISectorRepository sectorRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _jobApplicationsRepository = jobApplicationsRepository;
            _jobTypeRepository = jobTypeRepository;
            _sectorRepository = sectorRepository;
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
        public async Task<ActionResult> ViewApplication([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone,JobId,Status,UserId")] JobApplications jobApplications)
        {
            if (ModelState.IsValid)
            {
                await _jobApplicationsRepository.Update(jobApplications);
                await _jobApplicationsRepository.Save();
                return RedirectToAction("Index");
            }

            return View(jobApplications);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateJobStatus(UpdateJobDto model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Get", new { Id = model.Id });
            }

            var currentJob = await _jobRepository.GetById(model.Id);
            if (currentJob == null)
            {
                return HttpNotFound();
            }
            currentJob.Status = model.Status;
            var result = await _jobRepository.Update(currentJob);
            if (result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.success = "Updated job status";
            return RedirectToAction("Get", new { Id = model.Id });
        }

        [HttpGet]
        public async Task<ActionResult> ViewJobApplications(int Id)
        {
            var applications = await _jobApplicationsRepository.Get(c => c.Id == Id);
            applications = applications.Include("Job");
            return View(applications);
        }

        /*[HttpPost]
        public async Task<ActionResult> UpdateApplicantStatus(UpdateJobApplicantDto model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Get", new { Id = model.JobId });
            }

            var currentJob = await _jobRepository.GetById(model.JobId);
            if (currentJob == null)
            {
                return HttpNotFound();
            }

            var currentApplicant = currentJob.Applicants.FirstOrDefault(X => X.Id == model.ApplicantId);
            if (currentApplicant == null)
            {
                return HttpNotFound();
            }
            currentApplicant.Status = model.Status;
            currentApplicant.UpdatedAt = DateTime.UtcNow;
            var result = await _jobRepository.Update(currentJob);
            if (result == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ViewBag.success = "updated job status";
            return RedirectToAction("Get", new { Id = model.JobId });
        }*/
    }
}