using System.Linq;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Nancy;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class IndexModule : EmailRModule
    {
        public IndexModule(IDocumentSession documentSession)
        {
            Get["/"] = parameters =>
            {
                if (IsAuthenticated)
                {
                    var user =
                        documentSession.Load<EmailUser>(Principal.GetUserId());
                    return user.Organization == null ? View["noOrganization"] : View["index"];
                }
                return Response.AsRedirect("/login");
            };
        }
    }
}