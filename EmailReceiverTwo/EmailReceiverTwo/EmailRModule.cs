using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using EmailReceiverTwo.Helpers;
using EmailReceiverTwo.Models;
using Nancy;
using Nancy.Validation;

namespace EmailReceiverTwo
{
    public class EmailRModule : NancyModule
    {
        protected PageModel Page { get; set; }
        public dynamic Model = new ExpandoObject();

        public EmailRModule() : base()
        {
            Setup();
        }

        public EmailRModule(string modulePath)
            : base(modulePath)

        {
            Setup();
        }

        private void Setup()
        {
            Before += ctx =>
            {
                Page = new PageModel
                {
                    Errors = new List<ErrorModel>()
                };
                Model.Page = Page;
                return null;
            };
        }

        protected ClaimsPrincipal Principal
        {
            get { return this.GetPrincipal(); }
        }

        protected bool IsAuthenticated
        {
            get { return this.IsAuthenticated(); }
        }

        protected void SaveErrors(IEnumerable<ModelValidationError> errors)
        {
            foreach (var error in errors)
            {
                foreach (var member in error.MemberNames)
                {
                    Page.Errors.Add(new ErrorModel
                    {
                        ErrorMessage = error.GetMessage(member),
                        Name = member
                    });
                }
            }
        }
    }
}