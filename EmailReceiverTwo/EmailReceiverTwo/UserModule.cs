using System;
using System.Linq;
using EmailReceiver.Models;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Infrastructure.User;
using EmailReceiverTwo.Services;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class UserModule : EmailRModule
    {
        private readonly ICryptoService _crypto;

        public UserModule(IDocumentSession documentSession, ICryptoService crypto) : base("user")
        {
            Get["/"] = _ =>
            {
                if (IsAuthenticated)
                {
                    var user =
                        documentSession.Load<EmailUser>(Principal.GetUserId());
                    var users =
                        documentSession.Query<EmailUser>()
                            .Where(u => u.Organization.Id == user.Organization.Id)
                            .Select(c => new UserViewModel {Name = c.Name, Organization = c.Organization.Name}).ToList();
                    return Response.AsJson(users);
                }
                return HttpStatusCode.Unauthorized;
            };
            Post["/"] = _ =>
            {
                if (IsAuthenticated)
                {
                    var user =
                        documentSession.Load<EmailUser>(Principal.GetUserId());
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
                    documentSession.Store(newUser);
                    documentSession.SaveChanges();
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.Unauthorized;
            };
        }
    }
}