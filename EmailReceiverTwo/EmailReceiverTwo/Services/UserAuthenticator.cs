using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Raven.Client;

namespace EmailReceiverTwo.Services
{
    public class UserAuthenticator : IUserAuthenticator
    {
        private readonly IMembershipService _service;
        private readonly IDocumentSession _documentSession;

        public UserAuthenticator(IMembershipService service, IDocumentStore store)
        {
            _service = service;
            _documentSession = store.OpenSession();
        }

        public bool TryAuthenticateUser(string username, string password, out IList<Claim> claims)
        {
            claims = new List<Claim>();
            EmailUser user;
            if (_service.TryAuthenticateUser(username, password, out user))
            {
                claims.Add(new Claim(EmailRClaimTypes.Identifier, user.Id));
                if (user == user.Organization.Admin)
                    claims.Add(new Claim(EmailRClaimTypes.Admin, user.Id));
                return true;
            }
            return false;
        }
    }
}