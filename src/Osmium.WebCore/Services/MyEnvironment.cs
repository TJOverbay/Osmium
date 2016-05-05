using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Osmium.WebCore.Services
{
    public sealed class MyEnvironment
    {
        private readonly IHostingEnvironment _host;
        private readonly ApplicationEnvironmentWrapper _app;

        public MyEnvironment(IApplicationEnvironment app, IHostingEnvironment host)
        {
            _app = new ApplicationEnvironmentWrapper(app);
            _host = host;
        }

        public IDefaultObjectAccessor GlobalData { get { return _app; } }
        public string ApplicationName { get { return _app.ApplicationEnvironment.ApplicationName; } }
        public string ApplicationVersion { get { return _app.ApplicationEnvironment.ApplicationVersion; } }
        public string Framework { get { return _app.ApplicationEnvironment.RuntimeFramework.FullName; } }
        public string HostEnvironment { get { return _host.EnvironmentName; } }
        public bool IsDevelopment { get { return _host.IsDevelopment(); } }
        public bool IsStaging { get { return _host.IsStaging(); } }
        public bool IsProduction { get { return _host.IsProduction(); } }
        public IFileProvider WebRoot { get { return _host.WebRootFileProvider; } }
        public string WebRootPath { get { return _host.WebRootPath; } }

        public string MapPath(string virtualPath)
        {
            return _host.MapPath(virtualPath);
        }

        private class ApplicationEnvironmentWrapper : IDefaultObjectAccessor
        {
            public ApplicationEnvironmentWrapper(IApplicationEnvironment applicationEnvironment)
            {
                ApplicationEnvironment = applicationEnvironment;
            }
            public readonly IApplicationEnvironment ApplicationEnvironment;

            public object this[string key]
            {
                get
                {
                    return ApplicationEnvironment.GetData(key);
                }

                set
                {
                    ApplicationEnvironment.SetData(key, value);
                }
            }
        }
    }

    public interface IDefaultObjectAccessor
    {
        object this[string key] { get; set; }
    }
}
