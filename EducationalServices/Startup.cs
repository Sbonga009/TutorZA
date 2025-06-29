using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EducationalServices.Startup))]
namespace EducationalServices
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
