using System.Configuration;
using System.Threading;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Middleware;
using EmailReceiverTwo.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Nancy.Bootstrappers.Ninject;
using Nancy.Owin;
using Nancy.TinyIoc;
using Ninject;
using Owin;
using System;
using Raven.Client;
using Raven.Client.Document;

namespace EmailReceiverTwo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var kernel = SetupNinject();
            SetupAuth(app, kernel);
            SetupSignalR(app, kernel);
            SetupNancy(app, kernel);
        }

       

        private void SetupNancy(IAppBuilder app, IKernel kernel)
        {
            var bootstrapper = new EmailRNinjectNancyBootstrapper(kernel);
            var options = new NancyOptions();
            options.Bootstrapper = bootstrapper;
            app.UseNancy(options);
        }

        private void SetupAuth(IAppBuilder app, IKernel kernel)
        {
            var options = new CookieAuthenticationOptions
            {
                LoginPath = "/login",
                LogoutPath = "/logout",
                CookieHttpOnly = true,
                AuthenticationType = "EmailR",
                CookieName = "emailr.id",
                ExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = kernel.Get<ICookieAuthenticationProvider>()
            };

            app.UseCookieAuthentication(options);

            app.Use(typeof (WindowsPrincipalHandler));

            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private void SetupSignalR(IAppBuilder app, IKernel kernel)
        {

            var config = new HubConfiguration();
            var resolver = new NinjectSignalRDependencyResolver(kernel);
            var connectionManager = resolver.Resolve<IConnectionManager>();

            kernel.Bind<IConnectionManager>()
                  .ToConstant(connectionManager);
            config.Resolver = resolver;

            app.MapSignalR(config);
        }
        
        private KernelBase SetupNinject()
        {
            var store = new DocumentStore();
            var connectionString = ConfigurationManager.AppSettings["RavenDB"];
            store.ParseConnectionString(connectionString);
            store.Initialize();
           

            var kernel = new StandardKernel(new[] {new FactoryModule(),});
            kernel.Bind<IDocumentStore>()
                  .ToConstant(store).InSingletonScope();
            kernel.Bind<ICryptoService>()
                .To<CryptoService>();
            kernel.Bind<IKeyProvider>()
                .To<SettingsKeyProvider>();
            kernel.Bind<IMembershipService>()
                .To<MembershipService>();
            kernel.Bind<IUserAuthenticator>()
                .To<UserAuthenticator>();
            kernel.Bind<ICookieAuthenticationProvider>()
                .To<EmailRFormsAuthenticationProvider>();

            return kernel;
        }



    }
}