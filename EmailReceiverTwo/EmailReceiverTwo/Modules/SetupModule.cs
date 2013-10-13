using System;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Helpers;
using EmailReceiverTwo.Infrastructure;
using Nancy;
using Raven.Client;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Used to setup a new organization.
    /// </summary>
    public class SetupModule : EmailRModule
    {
        public SetupModule() : base("setup")
        {
            //Looks for the Organization query string and creates a new organization for the currently signed in user.
            Get["/"] = _ =>
            {
                string org = Request.Query.Organization;
                if (IsAuthenticated == false)
                    return Response.AsRedirect(string.Format("/login?returnUrl=/setup?Organization={0}",org));

                var user = DocumentSession.Load<EmailUser>(Principal.GetUserId());
                if (user.Organization != null)
                {
                    Page.ValidationSummary =
                        "You already belong to an organization.  If this is an error please contact your administrator";
                    return View["error", Model];
                }
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = org,
                    Admin = user
                };
                user.Organization = organization;
                DocumentSession.Store(organization);
                return this.AsRedirectQueryStringOrDefault("/");
            };
        }
    }
}