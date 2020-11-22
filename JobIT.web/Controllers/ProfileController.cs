using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobIT.web.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;
using JobIT.web.Respositories.UserDetailsRepos;
using System.Threading.Tasks;
using JobIT.web.Respositories.JobApplicationsRepo;

namespace JobIt.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IJobApplicationsRepository _jobApplicationsRepository;

        public ProfileController(IUserDetailsRepository userDetailsRepo, IJobApplicationsRepository jobApplicationsRepository)
        {
            _userDetailsRepository = userDetailsRepo;
            _jobApplicationsRepository = jobApplicationsRepository;
        }

        // GET: Dashboard
        public async Task<ActionResult> Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var userDetails = await _userDetailsRepository.GetSingleAsync(c => c.UserId == userId);
            return View(userDetails);
        }

        // GET: Profile/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userDetails = await _userDetailsRepository.GetById(id);
            if (userDetails == null)
            {
                return HttpNotFound();
            }
            return View(userDetails);
        }

        // GET: UserDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone")] UserDetails userDetails)
        {
            if (ModelState.IsValid)
            {
                userDetails.UserId = User.Identity.GetUserId();
                await _userDetailsRepository.AddAsync(userDetails);
                return RedirectToAction("Dashboard");
            }
            return View(userDetails);
        }

        // GET: Profile/Edit/5
        public async Task<ActionResult> Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userDetails = await _userDetailsRepository.GetById(id);
            if (userDetails == null)
            {
                return HttpNotFound();
            }
            return View(userDetails);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "Id,FirstName,LastName,Email,DateOfBirth,Address,City,State,Country,Phone,ApplicationUserId,UserId,ProfilePicPath")] UserDetails userDetails)
        {
            if (ModelState.IsValid)
            {
                await _userDetailsRepository.Update(userDetails);
                await _userDetailsRepository.Save();
                return RedirectToAction("Dashboard");
            }
            return View(userDetails);
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(UserDetails userDetails, HttpPostedFileBase File)
        {
            string filename = Path.GetFileNameWithoutExtension(File.FileName);
            string extension = Path.GetExtension(File.FileName);
            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

            userDetails.ProfilePicPath = "~/image/" + filename;

            var currentUser = await _userDetailsRepository.GetById(userDetails.Id);
            if (currentUser != null)
            {
                currentUser.ProfilePicPath = "~/image/" + filename;
                await _userDetailsRepository.Update(currentUser);
                filename = Path.Combine(Server.MapPath("~/image/"), filename);
                File.SaveAs(filename);
            }
            //await _userDetailsRepository.Save();
            ModelState.Clear();

            var result = "Successfully Uploaded";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ViewApplicationStatus()
        {
            var UserId = User.Identity.GetUserId();
            var applications = await _jobApplicationsRepository.Get(c => c.UserId == UserId);
            applications = applications.Include("Job");
            return View(applications);
        }

    }
}
