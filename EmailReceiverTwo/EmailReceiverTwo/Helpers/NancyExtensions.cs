using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Infrastructure.User;
using Microsoft.Owin;
using Nancy;
using Nancy.Owin;

namespace EmailReceiverTwo.Helpers
{
    public static class NancyExtensions
    {
        public static ClaimsPrincipal GetPrincipal(this NancyModule module)
        {
            var userIdentity = module.Context.CurrentUser as UserIdentity;

            if (userIdentity == null)
            {
                return null;
            }

            return userIdentity.ClaimsPrincipal;
        }

        public static Response SignIn(this NancyModule module, IEnumerable<Claim> claims, string returnUrl = "~/")
        {
            var env = Get<IDictionary<string, object>>(module.Context.Items, NancyOwinHost.RequestEnvironmentKey);
            var owinContext = new OwinContext(env);

            var identity = new ClaimsIdentity(claims, Constants.EmailRAuthType);
            owinContext.Authentication.SignIn(identity);

            return module.AsRedirectQueryStringOrDefault(returnUrl ?? "~/");
        }

        public static Response SignIn(this NancyModule module, EmailUser user, string returnUrl = "~/")
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(EmailRClaimTypes.Identifier, user.Id.ToString()));

            // Add the admin claim if the user is an Administrator
            if (user.IsAdmin)
            {
                claims.Add(new Claim(EmailRClaimTypes.Admin, "true"));
            }

            return module.SignIn(claims, returnUrl);
        }

        public static bool IsAuthenticated(this NancyModule module)
        {
            return module.GetPrincipal().IsAuthenticated();
        }


        public static Response AsRedirectQueryStringOrDefault(this NancyModule module, string defaultUrl)
        {
            string returnUrl = module.Request.Query.returnUrl;
            if (String.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = defaultUrl;
            }

            return module.Response.AsRedirect(returnUrl);
        }

        public static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }
    }
}