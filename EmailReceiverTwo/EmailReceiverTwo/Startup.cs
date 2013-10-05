using System.Threading;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;

namespace EmailReceiverTwo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = "/login",
                LogoutPath = "/logout",
                CookieHttpOnly = true,
                AuthenticationType = "EmailR",
                CookieName = "emailr.id",
                ExpireTimeSpan = TimeSpan.FromDays(30)
            });
            app.UseNancy();
        }
    }
}