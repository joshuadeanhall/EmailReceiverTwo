using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using EmailReceiverTwo.Infrastructure.User;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Owin;
using Nancy.Security;
using Ninject;
using Ninject.Activation;
using Raven.Client;

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

        protected override IKernel GetApplicationContainer()
        {
            return _kernel;
        }


      
        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Csrf.Enable(pipelines);

            pipelines.BeforeRequest.AddItemToStartOfPipeline(FlowPrincipal);
            pipelines.BeforeRequest.AddItemToEndOfPipeline(SetSession);
            pipelines.AfterRequest.AddItemToStartOfPipeline(DisposeSession);
        }

        private void DisposeSession(NancyContext context)
        {
            var env = Get<IDictionary<string, object>>(context.Items, NancyOwinHost.RequestEnvironmentKey);
            var session = Get<IDocumentSession>(env, "documentSession");
            session.SaveChanges();
            session.Dispose();
        }

        private Response SetSession(NancyContext context)
        {
            var env = Get<IDictionary<string, object>>(context.Items, NancyOwinHost.RequestEnvironmentKey);
            var session = Get<IDocumentSession>(env, "documentSession") as IDocumentSession;
            context.Items.Add("documentSession", session);
            return null;
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