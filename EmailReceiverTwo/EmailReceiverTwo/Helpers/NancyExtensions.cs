using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using EmailReceiverTwo.Infrastructure;
using EmailReceiverTwo.Infrastructure.User;
using Nancy;

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

        public static bool IsAuthenticated(this NancyModule module)
        {
            return module.GetPrincipal().IsAuthenticated();
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