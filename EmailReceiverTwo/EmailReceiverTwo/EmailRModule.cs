using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using EmailReceiverTwo.Helpers;
using Nancy;

namespace EmailReceiverTwo
{
    public class EmailRModule : NancyModule
    {
        public EmailRModule() : base()
        {

        }

        public EmailRModule(string modulePath)
            : base(modulePath)

        {

        }

        protected ClaimsPrincipal Principal
        {
            get { return this.GetPrincipal(); }
        }

        protected bool IsAuthenticated
        {
            get { return this.IsAuthenticated(); }
        }
    }
}