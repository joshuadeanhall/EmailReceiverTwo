using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Ninject;
using Raven.Client;

namespace EmailReceiverTwo.Middleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    public class RavenSessionHandler
    {
        private readonly AppFunc _next;
        private readonly IKernel _kernel;


        public RavenSessionHandler(AppFunc next, IKernel kernel)
        {
            _next = next;
            _kernel = kernel;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            var session = _kernel.Get<IDocumentStore>().OpenSession();
            _kernel.Bind<IDocumentSession>()
                .ToConstant(session)
                 .InSingletonScope();
            context.Set("documentSession", session);
            await _next(env);
        }
    }
}