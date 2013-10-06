using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EmailReceiver.Models;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Helpers;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Infrastructure.User;
using EmailReceiverTwo.Services;
using Microsoft.Owin;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Owin;
using Raven.Client;
using Raven.Client.Linq;

namespace EmailReceiverTwo
{
    public class LoginModule : EmailRModule
    {
        public LoginModule(IDocumentSession documentSession,
            ApplicationSettings applicationSettings,
                             IMembershipService membershipService)
        {
            Get["/login"] = _ =>
            {
                if (IsAuthenticated)
                {
                    return View["index"];
                }
                var model = new LoginClass
                {
                    ReturnUrl = Request.Query.returnUrl
                };
                return View["login", model];
            };

            Get["/logout"] = parameters =>
            {
                return View["login"];
            };

            Post["/login"] = parameters =>
            {
                if (IsAuthenticated)
                    return View["index"];
                var model = this.Bind<LoginClass>();

                var env = NancyExtensions.Get<IDictionary<string, object>>(Context.Items,
                    NancyOwinHost.RequestEnvironmentKey);
                var owinContext = new OwinContext(env);

                var claims = new List<Claim>();

                var userRecord =
                    documentSession.Query<EmailUser>()
                        .FirstOrDefault(x => x.Name == model.Username);
                if (userRecord != null)
                {
                    claims.Add(new Claim(EmailRClaimTypes.Identifier, userRecord.Id));
                    return this.SignIn(claims);
                }
                
                return View["login", model];
            };

            Get["/register"] = parameters =>
            {
                var registerModel = new RegisterViewModel();
                return View["Register", registerModel];
            };

            Post["/register"] = _ =>
            {
                var registerModel = this.Bind<RegisterViewModel>();
                var user = membershipService.AddUser(registerModel.UserName, registerModel.Email, registerModel.Password);
                return this.SignIn(user);
            };
        }

       
    }

    public class LoginClass
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}