using EmailReceiverTwo.Helpers;
using Nancy;
using Nancy.Security;

namespace EmailReceiverTwo
{
    public class IndexModule : EmailRModule
    {
        public IndexModule()
        {
            //v
            this.RequiresAuthentication();
            Get["/"] = parameters =>
            {
                var principal = this.GetPrincipal();
                var isAuth = this.IsAuthenticated;
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