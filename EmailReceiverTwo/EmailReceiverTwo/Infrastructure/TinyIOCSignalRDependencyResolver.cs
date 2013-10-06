using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Nancy.TinyIoc;

namespace EmailReceiverTwo.Infrastructure
{
    public class TinyIOCSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        public TinyIOCSignalRDependencyResolver(TinyIoCContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            object service;
            if (_container.TryResolve(serviceType, out service))
                return service;
            return  base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}