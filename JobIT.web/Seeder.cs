using JobIT.web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JobIT.web
{
    public class Seeder : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //context.Database.Delete();
            //context.Database.CreateIfNotExists();
            InitializeData(context);
            base.Seed(context);
        }

        private void InitializeData(ApplicationDbContext context)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var AdminUser = new ApplicationUser { Email = "Admin@test.com", EmailConfirmed = true, PhoneNumber = "+23455775833", UserName = "admin@test.com" };
            var client = new ApplicationUser { Email = "client@example.com", EmailConfirmed = true, PhoneNumber = "+23455775833", UserName = "client@example.com" };
            UserManager.Create(AdminUser, "1234567");
            RoleManager.Create(new IdentityRole { Name = "Admin" });
            UserManager.AddToRole(AdminUser.Id, "Admin");
            UserManager.Create(client, "1234567");

            var clientDetail = new UserDetails { FirstName = "Client", LastName = "Test", Email = "client@test.com", UserId = client.Id, DateOfBirth = DateTime.UtcNow };
            context.UserDetails.Add(clientDetail);

            var JobType = new JobType[]
            {
                new JobType {Name = "FullTime" },
                new JobType {Name = "Contract" },
                new JobType {Name = "Internship" }
            };
            var sector = new Sector[]
            {
                new Sector {Name = "Engineering" },
                new Sector {Name = "Technology" },
                new Sector {Name = "Health" }
            };
            context.Sector.AddRange(sector);
            context.JobType.AddRange(JobType);
            context.SaveChanges();

            var Jobs = new Job[]
            {
                new Job{ JobTypeId = JobType[0].Id, SectorId = sector[0].Id,  Title = "Lead Project Engineer", Description = "The Lead Project Engineer will be a key member of the Applied Engineering, Engineering Strategy & Integration team responsible to support the engineering strategy and driving the execution of footprint and transitions strategies for Aerospace engineering.", Responsibilities = "Lead team in execution of transition projects through all STP phases. As part of the Leadership team of ES&I, lead the execution of the Management Operating System to drive effective communication, interfacing, and execution of tasks & responsibilities between PSE, EOPS, GS&I, ISC, and SBU Engineering COEs. Lead the efforts associated with Engineering repositioning knowledge capture and management to ensure a disciplined and data-driven approach is utilized for assumptions and basis of estimate on all transition activities", Salary = "Negotiable", Company = "LabJab, Inc", Location = "Kentucky, USA"},
                new Job{ JobTypeId = JobType[1].Id, SectorId = sector[1].Id,  Title = "Software Developer", Description = "Software development", Responsibilities = "Writing code. Debuggin code. Managing code", Company = "Tenece", Location = "Lagos"},
                new Job{ JobTypeId = JobType[2].Id, SectorId = sector[1].Id, Title = "Software Tester", Description = "Software Testing", Responsibilities = "Writing code", Company = "Tenece", Location = "Lagos"},
                new Job{ JobTypeId = JobType[0].Id, SectorId = sector[1].Id, Title = "Product designer", Description = "Product Designer", Responsibilities = "Writing code", Company = "Tenece", Location = "Lagos"},
                new Job{ JobTypeId = JobType[0].Id, SectorId = sector[2].Id, Title = "Surgical Consultant", Description = "Be in charge of all surgical opertaions", Responsibilities = "Carrying out surgical operations. Operate as a consultant for difficult cases", Company = "NedKing, Inc", Location = "Bahamas"}
            };

            context.Jobs.AddRange(Jobs);
            context.SaveChanges();
        }
    }
}