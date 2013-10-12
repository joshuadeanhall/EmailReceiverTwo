using System.Collections.Generic;
using System.Dynamic;
using System.Security.Claims;
using EmailReceiverTwo.Helpers;
using EmailReceiverTwo.Models;
using Nancy;
using Nancy.Validation;
using Ninject;
using Raven.Client;

namespace EmailReceiverTwo.Modules
{
    /// <summary>
    /// Base module for EmailR.  
    /// </summary>
    public class EmailRModule : NancyModule
    {
        protected PageModel Page { get; set; }
        public dynamic Model = new ExpandoObject();
        public IDocumentSession DocumentSession { get; set; }

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
                DocumentSession = Context.Items["documentSession"] as IDocumentSession;
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