using System;
using System.Linq;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Helpers;
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
            Get["/"] = _ =>
            {
                string org = Request.Query.Organization;

                if (IsAuthenticated == false)
                    return Response.AsRedirect(string.Format("/login?returnUrl=/setup?Organization={0}",org));
                
                var user = documentSession.Load<EmailUser>(Principal.GetUserId());
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = org,
                    Admin = user
                };
                user.Organization = organization;
                documentSession.Store(organization);
                documentSession.SaveChanges();
                return this.AsRedirectQueryStringOrDefault("/");
            };
        }
    }
}