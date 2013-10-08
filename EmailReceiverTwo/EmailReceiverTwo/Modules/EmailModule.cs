﻿using System;
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
        public EmailModule(IDocumentSession documentSession, IConnectionManager connectionManager)
            : base("email")
        {
            //Returns emails for your organization that are not processed
            Get["/"] = _ =>
            {
                if (IsAuthenticated)
                {
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

            //Process an email
            //TODO make sure the user has access to the email.
            Post["/process/{Id}"] = parameters =>
            {
                if (IsAuthenticated)
                {
                    var email = documentSession.Load<Email>((Guid) parameters.Id);
                    email.Processed = true;
                    //documentSession.SaveChanges();
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
                }
                return HttpStatusCode.Unauthorized;
            };
            //Create an email.
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
                var hub = connectionManager.GetHubContext<EmailHub>();
                email.Id = orgEmail.Id;
                hub.Clients.Group(organization.Name).AddEmail(email);
                return HttpStatusCode.OK;
            };
        }
    }
}