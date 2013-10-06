using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace EmailReceiverTwo.Domain
{
    [JsonObject(IsReference = true)] 
    public class EmailUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Email { get; set; }
        public string LoginType { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int Status { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsAdmin { get; set; }
        public Organization Organization { get; set; }
        public List<EmailUserIdentity> Identities { get; set; }
        public string RequestPasswordResetId { get; set; }
        public DateTimeOffset? RequestPasswordResetValidThrough { get; set; }
    }
}
