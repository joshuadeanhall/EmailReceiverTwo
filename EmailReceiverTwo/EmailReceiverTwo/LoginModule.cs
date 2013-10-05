using System;
using System.Collections.Generic;
using System.Security.Claims;
using EmailReceiver.Models;
using EmailReceiverTwo.Infrastructure.User;
using Microsoft.Owin;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Owin;
using Raven.Client;

namespace EmailReceiverTwo
{
    public class LoginModule : NancyModule
    {
        public LoginModule(IDocumentSession documentSession)
        {
            Get["/login"] = _ =>
            {
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
                var model = this.Bind<LoginClass>();
                var userMapper = new UserMapper(documentSession);
                Guid? userGuid = userMapper.ValidateUser(model.Username, model.Password);
                if (userGuid != null)
                {
                    var env = Get<IDictionary<string, object>>(Context.Items, NancyOwinHost.RequestEnvironmentKey);
                    var owinContext = new OwinContext(env);

                    var claims = new List<Claim>();
                    var identity = new ClaimsIdentity(claims, "EmailR");
                    owinContext.Authentication.SignIn(identity);

                    return View["/"];
                    // return this.LoginAndRedirect(userGuid.Value, fallbackRedirectUrl: model.ReturnUrl);
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
                var userMapper = new UserMapper(documentSession);
                var userGUID = userMapper.ValidateRegisterNewUser(registerModel);
                if (userGUID != null)
                {
                    //this.LoginAndRedirect(userGUID.Value, DateTime.Now.AddDays(7), "/");
                }
                return View["Register", registerModel];
            };
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }
    }

    public class LoginClass
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}