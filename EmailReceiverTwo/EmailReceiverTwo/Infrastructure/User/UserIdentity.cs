using System.Collections.Generic;
using Nancy.Security;

namespace EmailReceiverTwo.Infrastructure.User
{
    public class UserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public string FriendlyName { get; set; }
    }
}