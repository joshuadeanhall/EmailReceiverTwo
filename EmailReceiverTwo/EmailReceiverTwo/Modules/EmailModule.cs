using System;
using System.Linq;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Hubs;
using EmailReceiverTwo.Infrastructure;
using Microsoft.AspNet.SignalR.Infrastructure;
using Nancy;
using Nancy.ModelBinding;
using Raven.Client;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Responsible for returning email functions in the application
    /// </summary>
    public class EmailModule : EmailRModule
    {
        public EmailModule(IConnectionManager connectionManager)
            : base("email")
        {
            //Returns emails for your organization that are not processed
            Get["/"] = _ =>
            {
                if (!IsAuthenticated)
                {
                    return HttpStatusCode.Unauthorized;
                }
                var user =
                        DocumentSession.Load<EmailUser>(Principal.GetUserId());
                    var emails =
                        DocumentSession.Query<Email>()
                            .Where(e => e.Organization.Id == user.Organization.Id && e.Processed == false)
                            .Select(e => new EmailViewModel
                            {
                                Id = e.Id,
                                Body = e.Body,
                                Domain = e.Organization.Name,
                                From = e.From,
                                Subject = e.Subject,
                                To = e.To
                            });
                    return Response.AsJson(emails);
            };

            //Process an email
            Post["/process/{Id}"] = parameters =>
            {
                if (!IsAuthenticated)
                {
                    return HttpStatusCode.Unauthorized;
                }
                var user =
                        DocumentSession.Load<EmailUser>(Principal.GetUserId());

                    var email = DocumentSession.Load<Email>((Guid)parameters.Id);
                    //Verify the user is in the same orginazation as the email being processed.
                    if (email.Organization != user.Organization)
                        return HttpStatusCode.Unauthorized;
                    email.Processed = true;
                    var hub = connectionManager.GetHubContext<EmailHub>();
                    var emailViewModel = new EmailViewModel
                    {
                        Id = email.Id,
                        Body = email.Body,
                        Domain = email.Organization.Name,
                        From = email.From,
                        Subject = email.Subject,
                        To = email.To

                    };
                    hub.Clients.Group(emailViewModel.Domain).EmailRemoved(emailViewModel);
                    return HttpStatusCode.OK;
            };
            //Create an email.
            Post["/"] = _ =>
            {
                var email = this.Bind<EmailViewModel>();
                var organization = DocumentSession.Query<Organization>().Single(o => o.Name == email.Domain);
                var orgEmail = new Email
                {
                    Body = email.Body,
                    Create = DateTime.Now,
                    From = email.From,
                    Id = Guid.NewGuid(),
                    Organization = organization,
                    Subject = email.Subject,
                    To = email.To
                };
                DocumentSession.Store(orgEmail);
                var hub = connectionManager.GetHubContext<EmailHub>();
                email.Id = orgEmail.Id;
                hub.Clients.Group(organization.Name).AddEmail(email);
                return HttpStatusCode.OK;
            };
        }
    }
}