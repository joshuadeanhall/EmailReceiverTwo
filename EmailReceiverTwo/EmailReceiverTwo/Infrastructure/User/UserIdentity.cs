using System.Collections.Generic;
using System.Security.Claims;
using Nancy.Security;

namespace EmailReceiverTwo.Infrastructure.User
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(ClaimsPrincipal claimsPrincipal)
        {
            ClaimsPrincipal = claimsPrincipal;
        }

        public ClaimsPrincipal ClaimsPrincipal { get; private set; }

        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public string FriendlyName { get; set; }
    }
}