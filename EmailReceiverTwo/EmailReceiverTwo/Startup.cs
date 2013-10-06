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
using Nancy.TinyIoc;
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
            var container = TinyIoCContainer.Current;

            var store = new DocumentStore();
            var connectionString = ConfigurationManager.AppSettings["RavenDB"];
            store.ParseConnectionString(connectionString);
            store.Initialize();
            container.Register(store, "DocStore");
            container.Register<ICryptoService, CryptoService>();
            container.Register<IKeyProvider, SettingsKeyProvider>();
            container.Register<IMembershipService, MembershipService>();
            container.Register<IUserAuthenticator, UserAuthenticator>();
            var docStore = container.Resolve<DocumentStore>("DocStore");
            var documentSession = docStore.OpenSession();
            container.Register<IDocumentSession>(documentSession);
            container.Register<ICookieAuthenticationProvider, EmailRFormsAuthenticationProvider>();
           
            SetupAuth(app, container);
            SetupSignalR(app, container);
            
            
            app.UseNancy();
        }

        private void SetupAuth(IAppBuilder app, TinyIoCContainer container)
        {
            var options = new CookieAuthenticationOptions
            {
                LoginPath = "/login",
                LogoutPath = "/logout",
                CookieHttpOnly = true,
                AuthenticationType = "EmailR",
                CookieName = "emailr.id",
                ExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = container.Resolve<ICookieAuthenticationProvider>()
            };

            app.UseCookieAuthentication(options);

            app.Use(typeof (WindowsPrincipalHandler));

            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private void SetupSignalR(IAppBuilder app, TinyIoCContainer container)
        {


            var config = new HubConfiguration();
            config.Resolver = new TinyIOCSignalRDependencyResolver(container);
            app.MapSignalR(config);
            //app.MapSignalR(config);

        }
    }
}