using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Infrastructure.User;
using EmailReceiverTwo.Services;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Owin;
using Nancy.Security;
using Nancy.TinyIoc;
using Ninject;
using Raven.Client;
using Raven.Client.Document;

namespace EmailReceiverTwo
{
    using Nancy;

    public class EmailRNinjectNancyBootstrapper : NinjectNancyBootstrapper
    {
        private readonly IKernel _kernel;

        public EmailRNinjectNancyBootstrapper(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Bind<IDocumentSession>()
                     .ToMethod(c => c.Kernel.Get<IDocumentStore>().OpenSession())
                     .InSingletonScope();
        }

        protected override IKernel GetApplicationContainer()
        {
            return _kernel;
        }


      
        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Csrf.Enable(pipelines);

            pipelines.BeforeRequest.AddItemToStartOfPipeline(FlowPrincipal);
        }

        private Response FlowPrincipal(NancyContext context)
        {
            var env = Get<IDictionary<string, object>>(context.Items, NancyOwinHost.RequestEnvironmentKey);
            if (env != null)
            {
                var principal = Get<IPrincipal>(env, "server.User") as ClaimsPrincipal;
                if (principal != null)
                {
                    context.CurrentUser = new UserIdentity(principal);
                }

                var appMode = Get<string>(env, "host.AppMode");

                if (!String.IsNullOrEmpty(appMode) &&
                    appMode.Equals("development", StringComparison.OrdinalIgnoreCase))
                {
                    context.Items["_debugMode"] = true;
                }
                else
                {
                    context.Items["_debugMode"] = false;
                }
            }

            return null;
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }
    }
}