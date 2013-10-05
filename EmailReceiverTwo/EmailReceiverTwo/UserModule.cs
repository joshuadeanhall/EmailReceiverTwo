using System;
using System.Linq;
using EmailReceiver.Models;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure.User;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class UserModule : NancyModule
    {
        public UserModule(IDocumentSession documentSession) : base("user")
        {
            this.RequiresAuthentication();
            Get["/"] = _ =>
            {
                var user =
                    documentSession.Query<UserModel>().Single(u => u.Username == this.Context.CurrentUser.UserName);
                var users =
                    documentSession.Query<UserModel>()
                        .Where(u => u.Organization.Id == user.Organization.Id)
                        .Select(c => new UserViewModel {Name = c.Username, Organization = c.Organization.Name}).ToList();
                return Response.AsJson(users);
            };
            Post["/"] = _ =>
            {
                 var user =
                    documentSession.Query<UserModel>().Single(u => u.Username == this.Context.CurrentUser.UserName);
                var createUser = this.Bind<CreateUserViewModel>();
                var newUser = new UserModel
                {
                    EmailAddress = createUser.Email,
                    FriendlyName = createUser.FriendlyName,
                    Id = Guid.NewGuid(),
                    LoginType = "Default",
                    Organization = user.Organization,
                    Password = UserMapper.EncodePassword(createUser.Password),
                    Username = createUser.UserName
                };
                documentSession.Store(newUser);
                documentSession.SaveChanges();
                return View["index"];
            };
        }
    }
}