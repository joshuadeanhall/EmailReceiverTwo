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
                             IMembershipService membershipService,
                               IUserAuthenticator authenticator)
        {
            Get["/login"] = _ =>
            {
                string returnUrl = Request.Query.returnUrl;
                if (IsAuthenticated)
                {
                    return Response.AsRedirect(returnUrl);
                }
                var model = new LoginViewModel
                {
                    ReturnUrl = returnUrl
                };
                return View["login", model];
            };

            Get["/logout"] = parameters =>
            {
                return View["login"];
            };

            Post["/login"] = parameters =>
            {
                var model = this.Bind<LoginViewModel>();

                if (IsAuthenticated)
                    return Response.AsRedirect(model.ReturnUrl);

                IList<Claim> claims;
                if (authenticator.TryAuthenticateUser(model.Username, model.Password, out claims))
                {
                    return this.SignIn(claims, model.ReturnUrl);
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
}