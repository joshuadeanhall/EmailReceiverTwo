using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace EmailReceiverTwo.Infrastructure
{
    public static class PrincipalExtensions
    {
        public static bool IsAuthenticated(this IPrincipal principal)
        {
            string userId = GetUserId(principal);

            return !String.IsNullOrEmpty(userId);
        }


        public static bool HasClaim(this ClaimsPrincipal principal, string type)
        {
            return !String.IsNullOrEmpty(principal.GetClaimValue(type));
        }

        public static string GetClaimValue(this ClaimsPrincipal principal, string type)
        {
            Claim claim = principal.FindFirst(type);

            return claim != null ? claim.Value : null;
        }

        public static string GetIdentityProvider(this ClaimsPrincipal principal)
        {
            return principal.GetClaimValue(ClaimTypes.AuthenticationMethod) ??
                   principal.GetClaimValue(AcsClaimTypes.IdentityProvider);
        }

        public static bool HasRequiredClaims(this ClaimsPrincipal principal)
        {
            return principal.HasClaim(ClaimTypes.NameIdentifier) &&
                   principal.HasClaim(ClaimTypes.Name) &&
                   !String.IsNullOrEmpty(principal.GetIdentityProvider());
        }

        public static bool HasPartialIdentity(this ClaimsPrincipal principal)
        {
            return !String.IsNullOrEmpty(principal.GetClaimValue(EmailRClaimTypes.PartialIdentity));
        }

        public static string GetUserId(this IPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                foreach (var identity in claimsPrincipal.Identities)
                {
                    if (identity.AuthenticationType == Constants.eamilRAuthType)
                    {
                        Claim idClaim = identity.FindFirst(EmailRClaimTypes.Identifier);

                        if (idClaim != null)
                        {
                            return idClaim.Value;
                        }
                    }
                }
            }
            return null;
        }
        public static string EncodePassword(string originalPassword)
        {
            if (originalPassword == null)
                return String.Empty;

            //Declarations

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            var md5 = new MD5CryptoServiceProvider();
            var originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            var encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }
    }
}