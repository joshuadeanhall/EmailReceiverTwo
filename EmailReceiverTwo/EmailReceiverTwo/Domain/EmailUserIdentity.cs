using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailReceiverTwo.Domain
{
    public class EmailUserIdentity
    {

        public Guid Key { get; set; }

        public int UserKey { get; set; }
        public EmailUser User { get; set; }

        public string Email { get; set; }
        public string Identity { get; set; }
        public string ProviderName { get; set; }
    }
}