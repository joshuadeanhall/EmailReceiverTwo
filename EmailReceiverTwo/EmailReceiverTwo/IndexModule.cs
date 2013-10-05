using System.Linq;
using EmailReceiverTwo.Domain;
using Nancy;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            this.RequiresAuthentication();
            Get["/"] = parameters =>
            {
                var currentUser = this.Context.CurrentUser;
                //var user = documentSession.Query<UserModel>().Single(u => u.Username == currentUser.UserName);
                //if (user.Organization == null)
                //{
                //    return View["noOrganization"];
                //}
                return View["index"];
            };
        }
    }
}