using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobIT.web.Startup))]
namespace JobIT.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
