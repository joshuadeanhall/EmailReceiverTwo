using System.Collections.Generic;
using System.Security.Claims;
using EmailReceiver.Models;
using EmailReceiverTwo.Helpers;
using EmailReceiverTwo.Services;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Handles the login, logout, and registering.
    /// </summary>
    public class LoginModule : EmailRModule
    {
        public LoginModule(IMembershipService membershipService,
                               IUserAuthenticator authenticator)
        {
            //Get the login page.  
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

                Model.LoginModel = model;
                return View["login", Model];
            };

            Get["/logout"] = parameters =>
            {
                return View["login"];
            };

            //Login.
            Post["/login"] = parameters =>
            {
                var model = this.Bind<LoginViewModel>();

                //If user is already authenticated redirect them to the returnUrl.
                if (IsAuthenticated)
                    return Response.AsRedirect(model.ReturnUrl);

                var result = this.Validate(model);

                if (!result.IsValid)
                {
                    SaveErrors(result.Errors);
                    Model.LoginModel = model;
                    return View["login", Model];
                }

                IList<Claim> claims;
                if (authenticator.TryAuthenticateUser(model.Username, model.Password, out claims))
                {
                    return this.SignIn(claims, model.ReturnUrl);
                }
                Page.ValidationSummary = "Your username or password was incorrect";
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
            //Register a new user.
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