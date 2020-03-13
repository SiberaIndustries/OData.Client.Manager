using System;
using System.Net.Http;

namespace OData.Client.Manager.Tests.Authenticators
{
    public class OidcAuthenticatorFixture :
#if NETCOREAPP3_1
        Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TestAuthorizationServer.Startup>,
#endif
        IDisposable
    {
        private bool disposed = false;

#if NETCOREAPP3_1
        public OidcAuthenticatorFixture()
        {
            ClientOptions.BaseAddress = new Uri("http://localhost:5000");
        }
#else
        public OidcAuthenticatorFixture()
        {
            var file = System.IO.Path.GetFullPath($"../../../../TestAuthorizationServer/bin/{(System.Diagnostics.Debugger.IsAttached ? "Debug" : "Release")}/netcoreapp3.1/TestAuthorizationServer.exe");
            Xunit.Assert.True(System.IO.File.Exists(file), "File doesn't exist: " + file);
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = file,
                    Arguments = "--useurls=http://localhost:5000"

                }
            };
            process.Start();
            Pid = process.Id;
        }

        public int Pid { get; private set; } = -1;
#endif

#if NETCOREAPP3_1
        public HttpClient Client => CreateClient();
#else
        public HttpClient Client => null;
#endif

#if NETCOREAPP3_1
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Client?.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }
#else
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Pid >= 0)
                    {
                        System.Diagnostics.Process.GetProcessById(Pid)?.Kill();
                        Pid = -1;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
#endif
    }
}
