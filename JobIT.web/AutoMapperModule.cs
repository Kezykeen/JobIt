using AutoMapper;
using JobIT.web.Dto;
using JobIT.web.Models;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobIT.web
{
    public class AutoMapperModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMapper>().ToMethod(AutoMapper).InSingletonScope();
        }

        private IMapper AutoMapper(Ninject.Activation.IContext context)
        {
            Mapper.Initialize(config =>
            {
                config.ConstructServicesUsing(type => context.Kernel.Get(type));

                config.CreateMap<Job, JobDto>().ReverseMap();
                config.CreateMap<Job, CreateJobDto>().ReverseMap();
                config.CreateMap<UserDetails, JobApplications>()
                .ForMember(dest => dest.Job, act => act.Ignore())
                .ForMember(dest => dest.JobId, act => act.Ignore())
                .ForMember(dest => dest.Status, act => act.Ignore());

                // .... other mappings, Profiles, etc.              

            });

            Mapper.AssertConfigurationIsValid(); // optional
            return Mapper.Instance;
        }
    }
}