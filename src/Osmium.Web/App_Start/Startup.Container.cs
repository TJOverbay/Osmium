using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Osmium.Web.Models;
using Owin;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;

// Register an httpmodule to create and dispose an excecution scope on each request
[assembly: PreApplicationStartMethod(typeof(Osmium.Web.ExecutionScopeHttpModule), "Initialize")]

namespace Osmium.Web
{
    public partial class Startup
    {
        public void ConfigureContainer(IAppBuilder app)
        {
            var container = CreateContainer(app);

            RegisterWebInfrastructure(container, Assembly.GetExecutingAssembly());

            container.Verify();

            SetDependencyResolver(container);
        }

        private Container CreateContainer(IAppBuilder app)
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            RegisterOwinServices(app, container);

            RegisterMyServices(container);

            return container;
        }

        private void RegisterMyServices(Container container)
        {
            // Add my container registrations here
            container.EnableHttpRequestMessageTracking(GlobalConfiguration.Configuration);

        }

        private void RegisterOwinServices(IAppBuilder app, Container container)
        {
            container.RegisterSingleton(app);
            container.Register<ApplicationUserManager>(Lifestyle.Scoped);
            container.Register(() => new ApplicationDbContext(null), Lifestyle.Scoped);
            container.Register<IUserStore<ApplicationUser>>(() =>
                new UserStore<ApplicationUser>(
                    container.GetInstance<ApplicationDbContext>()), Lifestyle.Scoped);
            container.RegisterInitializer<ApplicationUserManager>(
                manager => InitializeUserManager(manager, app));
            container.Register<SignInManager<ApplicationUser, string>, ApplicationSignInManager>(Lifestyle.Scoped);
            container.Register(() => AdvancedExtensions.IsVerifying(container)
                ? new OwinContext(new Dictionary<string, object>()).Authentication
                : HttpContext.Current.GetOwinContext().Authentication, Lifestyle.Scoped);

            app.CreatePerOwinContext(() => container.GetInstance<ApplicationUserManager>());
            app.CreatePerOwinContext(() => container.GetInstance<SignInManager<ApplicationUser, string>>());
        }

        private static void InitializeUserManager(ApplicationUserManager manager, IAppBuilder app)
        {
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = app.GetDataProtectionProvider();
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }

        private void RegisterWebInfrastructure(Container container, Assembly assembly)
        {
            container.RegisterMvcControllers(assembly);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.RegisterMvcIntegratedFilterProvider();
        }

        private void SetDependencyResolver(Container container)
        {
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
    }

    public class ExecutionScopeHttpModule : IHttpModule
    {
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(ExecutionScopeHttpModule));
        }

        void IHttpModule.Init(HttpApplication context)
        {
            var resolver = DependencyResolver.Current as SimpleInjectorDependencyResolver;

            context.BeginRequest += (sender, e) =>
            {
                HttpContext.Current.Items["osmium:scope"] = resolver.Container.BeginExecutionContextScope();
            };

            context.EndRequest += (sender, e) =>
            {
                var scope = HttpContext.Current.Items["scope"] as Scope;
                if (scope != null)
                {
                    scope.Dispose();
                }
            };
        }

        void IHttpModule.Dispose() { }
    }
}