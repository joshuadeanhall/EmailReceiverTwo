using System.Linq;
using EmailReceiver.Models;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Services;
using Nancy;
using Nancy.ModelBinding;
using Raven.Client;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Used for CRUD on an organizations users.
    /// </summary>
    public class UserModule : EmailRModule
    {
        public UserModule(ICryptoService crypto) : base("user")
        {
            //Get a list of all the users in the same organization as you.
            Get["/"] = _ =>
            {
                if (!IsAuthenticated)
                {
                    return HttpStatusCode.Unauthorized;
                }
                var user =
                    DocumentSession.Load<EmailUser>(Principal.GetUserId());
                var users =
                    DocumentSession.Query<EmailUser>()
                        .Where(u => u.Organization.Id == user.Organization.Id)
                        .Select(c => new UserViewModel {Name = c.Name, Organization = c.Organization.Name}).ToList();
                return Response.AsJson(users);
            };
            Post["/"] = _ =>
            {
                //If User is authenticated and is the org admin.
                if (IsAuthenticated && Principal.HasClaim(EmailRClaimTypes.Admin))
                {
                    return HttpStatusCode.Unauthorized;
                }
                var user =
                    DocumentSession.Load<EmailUser>(Principal.GetUserId());
                var createUser = this.Bind<CreateUserViewModel>();
                var salt = crypto.CreateSalt();
                var newUser = new EmailUser
                {
                    Email = createUser.Email,
                    FriendlyName = createUser.FriendlyName,
                    //Id = Guid.NewGuid(),
                    Password = createUser.Password.ToSha256(salt),
                    Salt = salt,
                    LoginType = "Default",
                    Organization = user.Organization,
                    Name = createUser.UserName,
                    IsAdmin = false
                };
                DocumentSession.Store(newUser);
                return HttpStatusCode.OK;
            };
        }
    }
}