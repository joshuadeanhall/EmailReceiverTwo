using System;
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
        //private readonly IDocumentSession _documentSession;

        //public EmailHub(IDocumentSession documentSession)
        //{
        //    _documentSession = documentSession;
        //}

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
           // var userId = Context.User.GetUserId();
           // var user = _documentSession.Load<EmailUser>(userId);
            Groups.Add(Context.ConnectionId, "test");
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
            Groups.Add(Context.ConnectionId, "test");
            return base.OnReconnected();
        }


        public string RemoveEmail(EmailViewModel emailViewModel)
        {
            Clients.All.EmailRemoved("subject1");
            return "Completed Op";
        }

        public void AddEmail(EmailViewModel email)
        {
            Clients.Group(email.Domain).AddEmail(email);
        }

        public void AddEmailTest()
        {
            string x = "test";

        }

    }
}