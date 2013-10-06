using System.Configuration;
using System.Threading;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Services;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
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

            var docStore = container.Resolve<DocumentStore>("DocStore");
            var documentSession = docStore.OpenSession();
            container.Register<IDocumentSession>(documentSession);
            container.Register<ICookieAuthenticationProvider, EmailRFormsAuthenticationProvider>();
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
            app.UseNancy();
        }
    }
}