using System;
using System.Linq;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Nancy;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class SetupModule : EmailRModule
    {
        public SetupModule(IDocumentSession documentSession) : base("setup")
        {
            Get["/{Organization}"] = parameters =>
            {
                if (IsAuthenticated == false)
                    return Response.AsRedirect("/login");

                var user = documentSession.Load<EmailUser>(Principal.GetUserId());
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = parameters.Organization,
                    Admin = user
                };
                user.Organization = organization;
                documentSession.Store(organization);
                documentSession.SaveChanges();
                return View["index"];
            };
        }
    }
}