using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Osmium.Web.Startup))]
namespace Osmium.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureContainer(app);
            ConfigureAuth(app);
        }
    }
}
