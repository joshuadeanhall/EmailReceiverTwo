using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailReceiverTwo.Infrastructure
{
    public static class Constants
    {
        public static readonly string AuthResultCookie = "emailr.authResult";
        public static readonly string emailRAuthType = "EmailR";
    }

    public static class EmailRClaimTypes
    {
        public const string Identifier = "urn:emailr:id";
        public const string Admin = "urn:emailr:admin";
        public const string PartialIdentity = "urn:emailr:partialid";
    }

    public static class AcsClaimTypes
    {
        public static readonly string IdentityProvider = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/IdentityProvider";
    }

    public static class ContentTypes
    {
        public const string Html = "text/html";
        public const string Text = "text/plain";
    }
}