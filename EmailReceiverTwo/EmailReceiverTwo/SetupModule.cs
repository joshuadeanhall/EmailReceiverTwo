using System;
using System.Linq;
using EmailReceiverTwo.Domain;
using Nancy;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class SetupModule : NancyModule
    {
        public SetupModule(IDocumentSession documentSession) : base("setup")
        {
            this.RequiresAuthentication();
            Get["/{Organization}"] = parameters =>
            {
                var user =
                    documentSession.Query<EmailUser>().Single(u => u.Name == this.Context.CurrentUser.UserName);
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