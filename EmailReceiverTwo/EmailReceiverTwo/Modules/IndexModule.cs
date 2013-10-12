﻿using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Nancy;
using Raven.Client;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Index module just returns the layout rest is handled by angular.
    /// </summary>
    public class IndexModule : EmailRModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                if (IsAuthenticated)
                {
                    var user =
                        DocumentSession.Load<EmailUser>(Principal.GetUserId());
                    return user.Organization == null ? View["noOrganization"] : View["index"];
                }
                return Response.AsRedirect("/login");
            };
        }
    }
}