using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Microsoft.Owin;
using Raven.Client;
using Raven.Client.Linq;

namespace EmailReceiverTwo.Infrastructure
{
    public class EmailRFormsAuthenticationProvider : ICookieAuthenticationProvider, IDisposable
    {
        private readonly IDocumentSession _session;
        private readonly IMembershipService _membershipService;

        public EmailRFormsAuthenticationProvider(IDocumentStore store, IMembershipService membershipService)
        {
            _session = store.OpenSession();
            _membershipService = membershipService;
        }

        public Task ValidateIdentity(CookieValidateIdentityContext context)
        {
            return TaskAsyncHelper.Empty;
        }

        public void ResponseSignIn(CookieResponseSignInContext context)
        {
            var authResult = new AuthenticationResult
            {
                Success = true
            };

            EmailUser loggedInUser = GetLoggedInUser(context);

            var principal = new ClaimsPrincipal(context.Identity);

            // Do nothing if it's authenticated
            if (principal.IsAuthenticated())
            {
                EnsurePersistentCookie(context);
                return;
            }

            var user = GetUser(principal);
            authResult.ProviderName = principal.GetIdentityProvider();

            // The user exists so add the claim
            if (user != null)
            {
                if (loggedInUser != null && user != loggedInUser)
                {
                    // Set an error message
                    authResult.Message = String.Format("This {0} account has already been linked to another user.", authResult.ProviderName);
                    authResult.Success = false;

                    // Keep the old user logged in
                    context.Identity.AddClaim(new Claim(EmailRClaimTypes.Identifier, loggedInUser.Id.ToString()));
                }
                else
                {
                    // Login this user
                    AddClaim(context, user);
                }

            }
            else if (principal.HasRequiredClaims())
            {
                EmailUser targetUser = null;

                // The user doesn't exist but the claims to create the user do exist
                if (loggedInUser == null)
                {
                    // New user so add them
                    user = _membershipService.AddUser(principal);

                    targetUser = user;
                }
                else
                {
                    // If the user is logged in then link
                    _membershipService.LinkIdentity(loggedInUser, principal);

                    _session.SaveChanges();

                    authResult.Message = String.Format("Successfully linked {0} account.", authResult.ProviderName);

                    targetUser = loggedInUser;
                }

                AddClaim(context, targetUser);
            }
            else if(!principal.HasPartialIdentity())
            {
                // A partial identity means the user needs to add more claims to login
                context.Identity.AddClaim(new Claim(EmailRClaimTypes.PartialIdentity, "true"));
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };

            context.Response.Cookies.Append(Constants.AuthResultCookie,
                                       JsonConvert.SerializeObject(authResult),
                                       cookieOptions);
        }

        private EmailUser GetUser(ClaimsPrincipal principal)
        {
            string identity = principal.GetClaimValue(ClaimTypes.NameIdentifier);
            var providerName = principal.GetIdentityProvider();

            var emailUserIdentity = _session.Query<EmailUserIdentity>().SingleOrDefault(u => u.Identity == identity && u.ProviderName == providerName);
            return emailUserIdentity != null ? emailUserIdentity.User : null;
        }

        private static void AddClaim(CookieResponseSignInContext context, EmailUser user)
        {
            // Add the jabbr id claim
            context.Identity.AddClaim(new Claim(EmailRClaimTypes.Identifier, user.Id.ToString()));

            // Add the admin claim if the user is an Administrator
            if (user.IsAdmin)
            {
                context.Identity.AddClaim(new Claim(EmailRClaimTypes.Admin, "true"));
            }

            EnsurePersistentCookie(context);
        }

        private static void EnsurePersistentCookie(CookieResponseSignInContext context)
        {
            if (context.Properties == null)
            {
                context.Properties = new AuthenticationProperties();
            }

            context.Properties.IsPersistent = true;
        }

        private EmailUser GetLoggedInUser(CookieResponseSignInContext context)
        {
            var principal = context.Request.User as ClaimsPrincipal;

            if (principal != null)
            {
                var userId = principal.GetUserId();
                return _session.Query<EmailUser>().SingleOrDefault(u => u.Id != null && u.Id == principal.GetUserId());
            }

            return null;
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}