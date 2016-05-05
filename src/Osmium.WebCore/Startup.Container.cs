using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Osmium.WebCore.Models;
using Osmium.WebCore.Services;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace Osmium.WebCore
{
    sealed partial class Startup
    {
        private readonly Container _container = new Container();

        void ConfigureContainer(IApplicationBuilder app, IHostingEnvironment env)
        {
            _container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            app.UseSimpleInjectorAspNetRequestScoping(_container);

            RegisterMyServices(app);

            //_container.Register<IUserRepository, SqlUserRepository>(Lifestyle.Scoped);
            _container.CrossWire<UserManager<ApplicationUser>>(app);
            _container.CrossWire<SignInManager<ApplicationUser>>(app);
            _container.CrossWire<ILoggerFactory>(app);
            _container.CrossWire<IApplicationEnvironment>(app);
            _container.RegisterSingleton(env);
            _container.RegisterAspNetControllers(app);
            _container.RegisterAspNetViewComponents(app);

            _container.Verify();
        }

        private void RegisterMyServices(IApplicationBuilder app)
        {
            _container.Register<IEmailSender, AuthMessageSender>();
            _container.Register<ISmsSender, AuthMessageSender>();
            _container.Register<MyEnvironment>();
        }
    }

}
