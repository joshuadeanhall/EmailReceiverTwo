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
using Nancy.Validation;
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

                base.Model.LoginModel = model;
                return View["login", Model];
            };

            Get["/logout"] = parameters =>
            {
                return View["login"];
            };

            Post["/login"] = parameters =>
            {
                
                var model = this.Bind<LoginViewModel>();

                var result = this.Validate(model);

                if (!result.IsValid)
                {
                    this.SaveErrors(result.Errors);
                    base.Model.LoginModel = model;
                    return View["login", base.Model];
                }

                if (IsAuthenticated)
                    return Response.AsRedirect(model.ReturnUrl);

                IList<Claim> claims;
                if (authenticator.TryAuthenticateUser(model.Username, model.Password, out claims))
                {
                    return this.SignIn(claims, model.ReturnUrl);
                }
                Model.LoginModel = model;
                return View["login", Model];
            };

            Get["/register"] = parameters =>
            {
                var registerModel = new RegisterViewModel();
                var returnUrl = Request.Query.returnUrl;
                registerModel.ReturnUrl = returnUrl;
                Model.RegisterModel = registerModel;
                return View["register", Model];
            };

            Post["/register"] = _ =>
            {
                var registerModel = this.Bind<RegisterViewModel>();
                Model.RegisterModel = registerModel;
                var result = this.Validate(registerModel);
                if (!result.IsValid)
                {
                    SaveErrors(result.Errors);
                    return View["register", Model];
                }
                var user = membershipService.AddUser(registerModel.UserName, registerModel.Email, registerModel.Password);
                return this.SignIn(user, registerModel.ReturnUrl);
            };
        }

       
    }
}