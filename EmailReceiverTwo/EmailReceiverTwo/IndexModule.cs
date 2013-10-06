using Nancy;

namespace EmailReceiverTwo
{
    public class IndexModule : EmailRModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                if (IsAuthenticated)
                {
                    //var user = documentSession.Query<UserModel>().Single(u => u.Username == currentUser.UserName);
                    //if (user.Organization == null)
                    //{
                    //    return View["noOrganization"];
                    //}
                    return View["index"];
                }
                return Response.AsRedirect("/login");
            };
        }
    }
}