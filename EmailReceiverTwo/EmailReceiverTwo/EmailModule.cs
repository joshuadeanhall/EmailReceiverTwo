using System;
using System.Linq;
using System.Reflection;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Hubs;
using EmailReceiverTwo.Infrastructure;
using Microsoft.AspNet.SignalR;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class EmailModule : EmailRModule
    {
        public EmailModule(IDocumentSession documentSession)
            : base("email")
        {
            
            Get["/"] = _ =>
            {
                if (IsAuthenticated)
                {
                    var userId = Principal.GetUserId();
                    var user =
                        documentSession.Load<EmailUser>(Principal.GetUserId());
                    var emails =
                        documentSession.Query<Email>()
                            .Where(e => e.Organization.Id == user.Organization.Id && e.Processed == false)
                            .Select(e => new EmailViewModel()
                            {
                                Id = e.Id,
                                Body = e.Body,
                                Domain = e.Organization.Name,
                                From = e.From,
                                Subject = e.Subject,
                                To = e.To
                            });
                    return Response.AsJson(emails);
                }
                return HttpStatusCode.Unauthorized;
            };

            Post["/process/{Id}"] = parameters =>
            {
                if (IsAuthenticated)
                {
                    var email = documentSession.Load<Email>((Guid) parameters.Id);
                    email.Processed = true;
                    //documentSession.SaveChanges();
                    var hub = GlobalHost.ConnectionManager.GetHubContext<EmailHub>();
                    var emailViewModel = new EmailViewModel
                    {
                        Id = email.Id,
                        Body = email.Body,
                        Domain = email.Organization.Name,
                        From = email.From,
                        Subject = email.Subject,
                        To = email.To

                    };

                    hub.Clients.All.EmailRemoved(emailViewModel);
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.Unauthorized;
            };

            Post["/"] = _ =>
            {
                var email = this.Bind<EmailViewModel>();
                var organization = documentSession.Query<Organization>().Single(o => o.Name == email.Domain);
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
                documentSession.Store(orgEmail);
                documentSession.SaveChanges();
               // var hub = GlobalHost.ConnectionManager.GetHubContext<EmailHub>();
                //hub.Clients.All.AddEmail(email);
                return HttpStatusCode.OK;
            };
        }
    }

    public class EmailViewModel
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Domain { get; set; }
    }
}