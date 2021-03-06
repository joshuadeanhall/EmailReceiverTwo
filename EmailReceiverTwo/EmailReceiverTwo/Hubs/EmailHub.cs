﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Microsoft.AspNet.SignalR;
using Raven.Client;


namespace EmailReceiverTwo.Hubs
{
    [AuthorizeClaim(EmailRClaimTypes.Identifier)]
    public class EmailHub : Hub
    {
        private readonly IDocumentStore _documentStore;

        public EmailHub(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            using (var session = _documentStore.OpenSession())
            {
                var userId = Context.User.GetUserId();
                var user = session.Load<EmailUser>(userId);
                Groups.Add(Context.ConnectionId, user.Organization.Name);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            using (var session = _documentStore.OpenSession())
            {
                var userId = Context.User.GetUserId();
                var user = session.Load<EmailUser>(userId);
                Groups.Add(Context.ConnectionId, user.Organization.Name);
            }
            return base.OnReconnected();
        }
    }
}