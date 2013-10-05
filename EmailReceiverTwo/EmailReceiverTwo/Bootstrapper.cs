﻿using System.Configuration;
using EmailReceiverTwo.Infrastructure.User;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Document;

namespace EmailReceiverTwo
{
    using Nancy;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
                 
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            var docStore = container.Resolve<DocumentStore>("DocStore");
            var documentSession = docStore.OpenSession();
            container.Register<IDocumentSession>(documentSession);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var store = new DocumentStore();
            var connectionString = ConfigurationManager.AppSettings["RavenDB"];
            store.ParseConnectionString(connectionString);
            store.Initialize();

            container.Register(store, "DocStore");
        }
    }
}